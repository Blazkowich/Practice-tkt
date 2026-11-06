"""
PostToolUse hook — fires after every Write or Edit tool call.
If the modified file is inside TKTDatabase/Schema Objects or
TKTDatabase/Scripts/Post-Deployment, reminds Claude to update
docs/TKTDatabase.md so the documentation stays in sync.
"""
import sys
import json

try:
    data = json.load(sys.stdin)
    path = data.get("file_path", "")
    if "TKTDatabase" in path and (
        "Schema Objects" in path or "Post-Deployment" in path
    ):
        print(
            "[hook] TKTDatabase modified — update docs/TKTDatabase.md "
            "if the schema or seed data changed."
        )
except Exception:
    pass
