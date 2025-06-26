# listener.py
import http.server, urllib.parse, threading

AUTH_CODE = ""

class Handler(http.server.BaseHTTPRequestHandler):
    def do_GET(self):
        print(f"👉 Received GET request: {self.path}")
        global AUTH_CODE
        parsed = urllib.parse.urlparse(self.path)
        query = urllib.parse.parse_qs(parsed.query)
        AUTH_CODE = query.get("code", [None])[0]
        print(f"👉 Received path: {self.path}")
        print(f"📥 Extracted auth code: {AUTH_CODE}")
        with open("auth_code.tmp", "w") as f:
            f.write(AUTH_CODE)
        self.send_response(200)
        self.end_headers()
        self.wfile.write(b"You may close this window.")
        threading.Thread(target=server.shutdown).start()

    def log_message(self, format, *args):
        return

server = http.server.HTTPServer(('localhost', 5555), Handler)
print("🚀 Python listener is live on port 5555")
server.serve_forever()
