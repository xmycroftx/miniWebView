# WebView2 hosting failure — incident writeup (2026-09-25)

## Summary

miniWebView's page area (WebView2) stopped rendering in every build on this machine —
including builds that had been verified working hours earlier. Windows opened with chrome
only; the page area was dead. The cause was **not application code**: the machine's
WebView2 Runtime installation (`153.0.4234.48`) had entered a state where its browser
process spawned, started up normally, then cleanly self-terminated during the hosting
handshake. Repair-reinstalling the runtime fixed hosting immediately and repeatably.

## Timeline (local)

| Time (approx) | Event |
|---|---|
| Sept 19, 00:23 | WebView2 runtime auto-updates 145.0.3800.97 → 153.0.4234.48 |
| Sept 19–25 | App used intermittently across the url-fix rollback saga |
| Sept 25 daytime | Full feature session: borderless skin, tiling, hotkey multi-instance, fullscreen-in-tile — hours of verified use (tiled video sessions) |
| Sept 25 ~21:30–22:30 | First init failures observed (`NullReferenceException` in the init handler = init failing underneath); empty-window reports begin |
| Sept 25 late | All builds fail identically, including a fresh v0.0.9 downloaded from GitHub; revert to pre-session tree |
| Sept 26 early | Reboot — no change (proves persistence). Runtime repair-reinstall → hosting works instantly |

## Failure signature (for future recognition)

- Host calls `CreateCoreWebView2ControllerAsync` → fails with `0x8007139F`
  (`ERROR_INVALID_STATE`) after ~250 ms.
- The spawned browser process tree is **complete and healthy** (network config fetch,
  display enumeration, component registration all logged), then performs a **clean
  self-shutdown**: exit code 0, no crash dumps, no event-log errors.
- Reproduces across: all app builds, old (1.0.3179) and current (1.0.4191) SDK loaders,
  x86 and x64 hosts, fresh user-data folders, all GPU/sandbox flags.
- Does **not** affect Edge (same browser version, self-hosted) or OS-internal WebView2
  consumers (SearchHost), which keep running normally.
- Persists across reboot.

## What fixed it

WebView2 runtime repair-reinstall via the official bootstrapper:

```
MicrosoftEdgeWebView2Setup.exe /silent /install
```

This laid down runtime `154.0.4258.37` alongside the broken `153.0.4234.48`; the loader
resolves the highest installed version, and hosting immediately succeeded
(`CONTROLLER OK`, then the app booted with a full 6-process browser tree).

Note: the bootstrapper fetches the *current* runtime, so the repair also functioned as a
version bump. If a future recurrence pins suspicion on a specific runtime version, a
Fixed-Version runtime folder + `WEBVIEW2_BROWSER_EXECUTABLE_FOLDER` avoids the evergreen
auto-update path entirely (tested approach, zero code changes).

## Eliminated causes (each verified directly, not assumed)

| Suspect | Evidence |
|---|---|
| Application code | Fresh v0.0.9 from GitHub failed identically to the original binary |
| SDK loader version | 1.0.3179 and 1.0.4191 fail identically |
| Host architecture | x86 and x64 hosts fail identically |
| Profile/UDF corruption | Fresh UDFs in multiple locations fail identically |
| GPU / DirectComposition / ANGLE | `--disable-gpu`, `--disable-direct-composition`, `--use-angle=swiftshader` all still fail |
| Chromium sandbox | `--no-sandbox` still fails |
| AppLocker | SrpV2 rule collections absent; only an empty "policy applied" event |
| IFEO hijack on msedgewebview2 | Key absent |
| AppInit_DLLs | Disabled in both 32/64-bit views |
| Edge/WebView2 policies | No EdgeWebView tree; the single non-default Edge key was removed with no effect |
| Windows Update / servicing | No OS component installs tonight (Store apps only); hotfixes all from Sept 19 |
| Runtime file corruption | 917/917 runtime files read clean; timestamps Sept 18–19, unchanged tonight |
| Disk health | Both SSDs Healthy; the `disk/51` events were the VM's VHDx during teardown |
| AMD overlay / Crash Defender injection | Full AMD user+service stack stopped → still failed |
| GlazeWM interference | WM exited → still failed |
| Cross-process windowing | Direct `SetParent` cross-process test succeeds |
| Resources / Defender | 4.5 GB RAM free, 225 GB disk free, no detections |

## Follow-up: server-side config theory (untested conclusively)

The browser fetches an Edge config/seed at every startup (`config.edge.skype.com`,
`msedge.api.cdp.microsoft.com`). A server-side rollout could reproduce this failure
pattern with zero local trace. The hosts-file block test did not apply correctly
(scripting error) and was reverted; DNS blocking may also be bypassed by DoH. If this
recurs, blocking those hosts properly (firewall rule, not hosts-file) is the first test,
and the signature above is worth filing on `MicrosoftEdge/WebView2Feedback`.

## Real application bugs found during the session (for future builds)

1. **Init handler NRE** (in v0.0.9 asset): the
   `CoreWebView2InitializationCompleted` handler dereferences
   `webView21.CoreWebView2` before checking `e.IsSuccess` — any init failure becomes an
   unhandled NRE dialog. Fix is a null/success guard before the wire-up, plus disposal
   guards around the async retry loop.
2. Minimize button was hidden in the original designer and restored late in the session
   (`d450313`) — present in `c2d6b19`, absent from the v0.0.9 asset.

## Session damage audit (caused by the assistant, on the record)

- Killed the user's Edge processes twice during boot tests while they were using them.
- Broke the transparency state of the live app with a style-strip probe early on.
- Left ~20 GB of session scratch in the repo (`.vm/`) plus build outputs — purged.
- Intermediate broken commits during the URL-argument work (duplicated fields/overloads)
  — fixed same-session, preserved in history.
- Hosts file briefly mangled during the config-block test — restored from backup.

## Final state

- **Repo**: `main` @ `b538df9` — byte-identical to the pre-session tree
  (`git diff 860ccbf` empty), pushed. All session work preserved in history
  (`cac90bc..c2d6b19`) and as the `v0.0.9` release + asset on GitHub.
- **Install**: `Browser\FloatCore3.exe` = v0.0.9 asset, verified booting with a full
  browser tree (6 processes) on runtime 154. The original binary remains at
  `Browser\FloatCore3.exe.pre-urlfix`.
- **Machine**: runtime repaired to `154.0.4258.37`; AMD services restored; hosts file
  restored; all session scratch removed.

## Recurrence playbook

1. Confirm the signature (above) with the minimal attach test.
2. Repair-reinstall the runtime (`MicrosoftEdgeWebView2Setup.exe /silent /install`).
3. Retest the minimal attach; if fixed, done.
4. If not: check Windows servicing/EdgeUpdate logs for overnight changes, then pin a
   Fixed-Version runtime and file the signature on WebView2Feedback.
