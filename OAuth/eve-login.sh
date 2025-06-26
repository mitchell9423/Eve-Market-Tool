#!/bin/bash
export PATH="/opt/homebrew/bin:/usr/local/bin:/usr/bin:/bin:/usr/sbin:/sbin"
echo "NODE PATH=$(which node || echo 'not found')"


sleep 2


# ==== SETTINGS ====
LOG_FILE="$1"
CLIENT_ID="$2"
CLIENT_SECRET="$3"
REDIRECT_URI="$4"
SCOPE="$5"
AUTH_URI="$6"
TOKEN_URL="$7"
CERT_FILE="$8"
KEY_FILE="$9"
TOKEN_FILE="${10}"
REFRESH_TOKEN="${11}"

STATE=$(uuidgen)  # Generate a unique random state string
AUTH_URL="${AUTH_URI}?response_type=code&client_id=${CLIENT_ID}&redirect_uri=${REDIRECT_URI}&scope=${SCOPE// /%20}&state=${STATE}"


# ==== LOGGING ====
exec > >(tee -i $LOG_FILE)
exec 2>&1


echo "LOG_FILE = ${LOG_FILE}"
echo "CLIENT_ID = ${CLIENT_ID}"
echo "CLIENT_SECRET = ${CLIENT_SECRET}"
echo "REDIRECT_URI = ${REDIRECT_URI}"
echo "AUTH_URI = ${AUTH_URI}"
echo "TOKEN_URL = ${TOKEN_URL}"
echo "CERT_FILE = ${CERT_FILE}"
echo "KEY_FILE = ${KEY_FILE}"
echo "REFRESH_TOKEN = ${REFRESH_TOKEN}"
echo "AUTH_URL = ${AUTH_URL}"


echo "🌐 Starting EVE SSO login flow..."


# ==== CHECK DEPENDENCIES ====
command -v node >/dev/null || { echo "❌ Node.js not found. Install it first."; exit 1; }
command -v npm >/dev/null || { echo "❌ npm not found. Install it first."; exit 1; }


# ==== CHECK SSL PROXY ====
if ! command -v local-ssl-proxy &> /dev/null; then
  echo "📦 Installing local-ssl-proxy..."
  npm install -g local-ssl-proxy
fi


# ==== CERTS ====
if [ ! -f "$CERT_FILE" ] || [ ! -f "$KEY_FILE" ]; then
  if ! command -v mkcert &> /dev/null; then
    echo "📦 Installing mkcert..."
    brew install mkcert
    mkcert -install
  fi
  echo "🔐 Generating certs..."
  mkcert -key-file "$KEY_FILE" -cert-file "$CERT_FILE" localhost
fi


# ==== VERIFYING CERTS ====
  echo "🔐 Validating Certs"
CERT_VERIFY=$(openssl x509 -noout -modulus -in "$CERT_FILE" | openssl md5)
KEY_VERIFY=$(openssl rsa  -noout -modulus -in "$KEY_FILE"  | openssl md5)
if [[ "$CERT_VERIFY" != "$KEY_VERIFY" ]]; then
  echo "❌ Replacing mismatch Cert/Key..."
  rm "$CERT_FILE" "$KEY_FILE"
  echo "🔐 Generating new certs..."
  mkcert -key-file "$KEY_FILE" -cert-file "$CERT_FILE" localhost
fi


# ==== LOAD PREVIOUS AUTH CODE ====
OLD_CODE=""
if [[ -f "auth_code.tmp" ]]; then
  OLD_CODE=$(cat auth_code.tmp)
fi


# ==== ENCODE CREDENTIALS ====
B64_AUTH=$(echo -n "$CLIENT_ID:$CLIENT_SECRET" | base64)


# ==== ATTEMPT TOKEN REFRESH FIRST ====
echo "🔁 Checking for existing refresh token..."
if [ -n "$REFRESH_TOKEN" ] && [ "$REFRESH_TOKEN" != "null" ]; then
    echo "🔐 Attempting to refresh token = $REFRESH_TOKEN"
    RESPONSE=$(curl -s -X POST "$TOKEN_URL" \
      -H "Authorization: Basic $B64_AUTH" \
      -H "Content-Type: application/x-www-form-urlencoded" \
      -d "grant_type=refresh_token&refresh_token=$REFRESH_TOKEN")

        if echo "$RESPONSE" | jq -e '.access_token' > /dev/null; then
          echo "✅ Token refreshed successfully."
          if [ -z "$RESPONSE" ]; then
          echo "❌ RESPONSE is empty, not writing token file."
        else
          echo "📝 TOKEN_FILE is: $TOKEN_FILE"
          echo "$RESPONSE" > "$TOKEN_FILE"
          echo "✅ Token written to $TOKEN_FILE"
        fi

        exit 0
    else
        echo "⚠️ Refresh failed, proceeding to full login..."
        echo "⚠️ Response: $RESPONSE"
    fi
else
    echo "❌ Refresh token not found"
fi


sleep 1


# ==== CLEANUP OLD PORTS ====
echo "🧹 Cleaning up old processes..."
lsof -ti :5555 | xargs kill -9 2>/dev/null
lsof -ti :8080 | xargs kill -9 2>/dev/null


# ==== PORT TIMEOUT FUNCTION ====
wait_for_port() {
  local port=$1
  local name=$2
  local timeout=10
  local waited=0

  echo "⏳ [$name] Waiting for port $port to become available..."
  while ! lsof -i :$port > /dev/null 2>&1; do
    echo "⏳ [$name] Still waiting... (${waited}s elapsed)"
    sleep 1
    waited=$((waited + 1))
    if [ $waited -ge $timeout ]; then
      echo "❌ [$name] Timeout! Port $port not available after ${timeout}s."
      exit 1
    fi
  done
  echo "✅ [$name] Port $port is listening."
}

# ==== HTTP READINESS FUNCTION ====
wait_for_http_ready() {
  local url=$1
  local name=$2
  local timeout=10
  local waited=0

  echo "🌐 [$name] Probing $url for HTTP response..."
  #while ! curl -ks "$url" > /dev/null 2>&1; do
  while ! nc -z localhost 8080; do
    echo "⏳ [$name] Still not ready... (${waited}s elapsed)"
    sleep 1
    waited=$((waited + 1))
    if [ $waited -ge $timeout ]; then
      echo "❌ [$name] Timeout! $url not responding after ${timeout}s."
      exit 1
    fi
  done
  echo "✅ [$name] $url is responsive."
}

wait_till_ready() {
  local url=$1
  local hostname=$2
  local port=$3
  local name=$4
  MAX_RETRIES=10
  RETRY_DELAY=1
  i=0

    echo "⏳ [$name] Probing $url for HTTP response..."
    while ! nc -z $hostname $port; do
        echo "⏳ [$name] Still not ready... (${i}s elapsed)"
        sleep $RETRY_DELAY
        i=$((i + RETRY_DELAY))
        if [ "$i" -ge "$MAX_RETRIES" ]; then
            echo "❌ [$name] Timeout! $url not responding after ${MAX_RETRIES}s."
            exit 1
        fi
    done

echo "✅ [$name] Ready!"

}

# ==== START LOCAL LISTENER ====
echo "👂 Starting Python listener in background..."
rm -f auth_code.tmp
python3 listener.py &
LISTENER_PID=$!
wait_for_port 5555 "Listener"
#wait_for_http_ready http://localhost:5555 "Listener"
wait_till_ready http://localhost:5555 localhost 5555 "Listener"

# ==== START SSL PROXY ====
echo "🚀 Launching local-ssl-proxy in background..."
DEBUG=* local-ssl-proxy --source 8080 --target 5555 --cert "$CERT_FILE" --key "$KEY_FILE" &
PROXY_PID=$!
wait_for_port 8080 "SSL Proxy"
#wait_for_http_ready https://localhost:8080 "SSL Proxy"
wait_till_ready https://localhost:8080 localhost 8080 "SSL Proxy"

# ==== CLEANUP ON EXIT ====
trap "echo '🧹 Cleaning up...'; kill $LISTENER_PID $PROXY_PID 2>/dev/null" EXIT


sleep 2


# ==== VERIFY LISTENING ====
if ! lsof -i :5555 > /dev/null; then
  echo "❌ Python listener failed to start on port 5555!"
  echo "Check python_server.log for details."
  cat python_server.log
  exit 1
fi


sleep 1


# ==== LAUNCH LOGIN IN BROWSER ====
echo "🌐 Opening browser for login..."
open -a "Google Chrome" "$AUTH_URL"


# ==== WAIT FOR CODE ====
echo "⏳ Waiting for auth code..."
while [ ! -f auth_code.tmp ]; do sleep 1; done
CODE=$(cat auth_code.tmp)


# ==== VERIFY FRESH AUTH CODE ====
if [[ "$CODE" == "$OLD_CODE" ]]; then
  echo "❌ Duplicate auth code detected. Aborting."
  exit 2
fi


# ==== REQUEST ACCESS TOKEN ====
echo "🔄 Exchanging auth code for access token..."
RESPONSE=$(curl -X POST "$TOKEN_URL" \
  -H "Authorization: Basic $B64_AUTH" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=authorization_code&code=$CODE&redirect_uri=$REDIRECT_URI")
echo "$RESPONSE" > token.json
echo "$CODE" > auth_code.tmp


sleep 1


# ==== SHUTDOWN ====
echo "🧹 Shutting down proxy..."
kill $PROXY_PID
echo "✅ Done. Output saved to $LOG_FILE"
exit 0
