$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName System.Drawing
Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type @"
using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
public class U32 {
    public delegate bool EnumProc(IntPtr h, IntPtr l);
    [DllImport("user32.dll")] public static extern bool SetProcessDPIAware();
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr h, out RECT r);
    [DllImport("user32.dll")] public static extern int GetWindowLongW(IntPtr h, int idx);
    [DllImport("user32.dll")] public static extern bool SetCursorPos(int x, int y);
    [DllImport("user32.dll")] public static extern void mouse_event(uint flags, uint dx, uint dy, uint data, UIntPtr extra);
    [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr h);
    [DllImport("user32.dll")] public static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")] public static extern bool IsWindow(IntPtr h);
    [DllImport("user32.dll")] public static extern bool SetWindowPos(IntPtr h, IntPtr after, int x, int y, int cx, int cy, uint flags);
    [DllImport("user32.dll")] public static extern bool EnumWindows(EnumProc cb, IntPtr l);
    [DllImport("user32.dll")] public static extern bool IsWindowVisible(IntPtr h);
    [DllImport("user32.dll")] public static extern uint GetWindowThreadProcessId(IntPtr h, out uint pid);
    [DllImport("user32.dll")] public static extern IntPtr WindowFromPoint(POINT p);
    [DllImport("user32.dll")] public static extern IntPtr GetAncestor(IntPtr h, uint flags);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] public static extern int GetClassNameW(IntPtr h, StringBuilder sb, int max);
    [StructLayout(LayoutKind.Sequential)] public struct RECT { public int L; public int T; public int R; public int B; }
    [StructLayout(LayoutKind.Sequential)] public struct POINT { public int X; public int Y; }
    public static List<IntPtr> WindowsOf(uint pid) {
        var list = new List<IntPtr>();
        EnumWindows(delegate(IntPtr hh, IntPtr l) {
            uint p; GetWindowThreadProcessId(hh, out p);
            if (p == pid && IsWindowVisible(hh)) { list.Add(hh); }
            return true;
        }, IntPtr.Zero);
        return list;
    }
    public static string ClassName(IntPtr h) { var sb = new StringBuilder(256); GetClassNameW(h, sb, 256); return sb.ToString(); }
}
"@
[U32]::SetProcessDPIAware() | Out-Null

$out = Join-Path $PSScriptRoot 'verify-out'
New-Item -ItemType Directory -Force -Path $out | Out-Null
$script:failCount = 0

function Shot([string]$name) {
    $vs = [System.Windows.Forms.SystemInformation]::VirtualScreen
    $bmp = New-Object System.Drawing.Bitmap($vs.Width, $vs.Height)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.CopyFromScreen($vs.X, $vs.Y, 0, 0, $bmp.Size)
    $g.Dispose()
    $pp = Join-Path $out $name
    $bmp.Save($pp, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
    Write-Output ("SHOT " + $pp)
}

function Check([string]$name, [bool]$ok, [string]$detail) {
    if ($ok) { Write-Output ("PASS " + $name + " (" + $detail + ")") }
    else { Write-Output ("FAIL " + $name + " (" + $detail + ")") }
    $script:failCount += (1 - [int]$ok)
}

function WaitUntil([scriptblock]$cond, [int]$ms) {
    $sw = [System.Diagnostics.Stopwatch]::StartNew()
    do { $rr = & $cond; if ($rr) { return $true }; Start-Sleep -Milliseconds 100 } while ($sw.ElapsedMilliseconds -lt $ms)
    return $false
}

$exe = Join-Path $PSScriptRoot 'FloatCore3\bin\Release\FloatCore3.exe'
if (-not (Test-Path $exe)) { throw ("missing exe: " + $exe) }
Get-Process FloatCore3 -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Milliseconds 500

$p = Start-Process -FilePath $exe -PassThru
$h = [IntPtr]::Zero
for ($i = 0; $i -lt 60; $i++) {
    Start-Sleep -Milliseconds 500
    $p.Refresh()
    if ($p.MainWindowHandle -ne [IntPtr]::Zero) { $h = $p.MainWindowHandle; break }
}
if ($h -eq [IntPtr]::Zero) { throw 'no main window handle' }
Start-Sleep -Milliseconds 3000
$w = [uint32]$p.Id

function GetRect {
    $r = New-Object U32+RECT
    [U32]::GetWindowRect($h, [ref]$r) | Out-Null
    return $r
}
function Assert-Alive([string]$when) {
    if (-not [U32]::IsWindow($h)) {
        $p.Refresh()
        Write-Output ("ABORT window dead " + $when + " exited=" + $p.HasExited + " exitcode=" + $(if ($p.HasExited) { $p.ExitCode } else { 'n/a' }))
        Get-Process FloatCore3 -ErrorAction SilentlyContinue | Stop-Process -Force
        exit 99
    }
}

# move to bottom-right corner, away from the user's active work
$wa = [System.Windows.Forms.Screen]::PrimaryScreen.WorkingArea
[U32]::SetWindowPos($h, [IntPtr]::Zero, ($wa.Right - 700), ($wa.Bottom - 650), 0, 0, 0x0001 -bor 0x0004) | Out-Null
Start-Sleep -Milliseconds 300

$r = GetRect
$scale = ($r.R - $r.L) / 648.0
Write-Output ("RECT L=" + $r.L + " T=" + $r.T + " R=" + $r.R + " B=" + $r.B + " scale=" + $scale + " pid=" + $w)

# click at client coords with fresh-rect recompute and post-click position verification
function Click-Client([double]$cx, [double]$cy, [string]$btn) {
    Assert-Alive ("before " + $btn + " click client(" + $cx + "," + $cy + ")")
    foreach ($attempt in 1..3) {
        $rr = GetRect
        $sc = ($rr.R - $rr.L) / 648.0
        $sx = [int]($rr.L + $cx * $sc)
        $sy = [int]($rr.T + $cy * $sc)
        # raise the app above other normal windows (HWND_TOP, not topmost) so the point is hittable
        [U32]::SetWindowPos($h, [IntPtr]::Zero, 0, 0, 0, 0, 0x0001 -bor 0x0004 -bor 0x0010) | Out-Null
        Start-Sleep -Milliseconds 120
        # never click into a foreign window: the app (or its own WebView2 child windows) must own the point
        $pt = New-Object U32+POINT; $pt.X = $sx; $pt.Y = $sy
        $under = [U32]::WindowFromPoint($pt)
        $upid = [uint32]0
        [U32]::GetWindowThreadProcessId($under, [ref]$upid) | Out-Null
        $root = [U32]::GetAncestor($under, 2)          # GA_ROOT
        $owner = [U32]::GetAncestor($root, 2)          # GA_ROOTOWNER
        if (($upid -ne $w) -and ($root -ne $h) -and ($owner -ne $h) -and ((GetRect).R -ne 0) -and ($root -ne (GetRect)) ) {
            $rr3 = GetRect
            $appRect = "$($rr3.L),$($rr3.T)"
            Write-Output ("SKIP click: point (" + $sx + "," + $sy + ") under class=" + [U32]::ClassName($under) + " pid=" + $upid + " root=" + $root + " appRect=" + $appRect)
            return
        }
        [U32]::SetCursorPos($sx, $sy) | Out-Null
        Start-Sleep -Milliseconds 80
        $down = 0x2; $up = 0x4
        if ($btn -eq 'right') { $down = 0x8; $up = 0x10 }
        [U32]::mouse_event($down, 0, 0, 0, [UIntPtr]::Zero)
        Start-Sleep -Milliseconds 50
        [U32]::mouse_event($up, 0, 0, 0, [UIntPtr]::Zero)
        Start-Sleep -Milliseconds 150
        $rr2 = GetRect
        if (([Math]::Abs($rr2.L - $rr.L) -le 2) -and ([Math]::Abs($rr2.T - $rr.T) -le 2)) {
            Write-Output ("CLICK " + $btn + " client(" + $cx + "," + $cy + ") -> screen(" + $sx + "," + $sy + ")")
            return
        }
        Write-Output ("CLICK moved (window dragged mid-click), retry " + $attempt)
    }
}

function Guarded-Keys([string]$keys) {
    $ok = WaitUntil { $fg = [U32]::GetForegroundWindow(); ($fg -eq $h) -or (($script:menuHwnd -ne [IntPtr]::Zero) -and ($fg -eq $script:menuHwnd)) } 2500
    if (-not $ok) { Write-Output 'KEYS skipped: neither app nor menu foreground'; return $false }
    [System.Windows.Forms.SendKeys]::SendWait($keys)
    Start-Sleep -Milliseconds 150
    return $true
}

function Find-MenuWindow {
    foreach ($hw in [U32]::WindowsOf($w)) {
        if ($hw -eq $h) { continue }
        $cn = [U32]::ClassName($hw)
        if ($cn -eq '#32768') { return $hw }
        if ($cn -notmatch '^WindowsForms10\.') { continue }
        $mr = New-Object U32+RECT
        [U32]::GetWindowRect($hw, [ref]$mr) | Out-Null
        $mw = $mr.R - $mr.L; $mh = $mr.B - $mr.T
        if (($mw -ge 60) -and ($mw -le 400) -and ($mh -ge 18) -and ($mh -le 250)) { return $hw }
    }
    return [IntPtr]::Zero
}

function Find-AotItemInMenu {
    if ($script:menuHwnd -eq [IntPtr]::Zero) { return $null }
    $el = [System.Windows.Automation.AutomationElement]::FromHandle($script:menuHwnd)
    if ($el -eq $null) { return $null }
    $items = $el.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
    foreach ($it in $items) {
        if ($it.Current.Name -eq 'Always on Top') { return $it }
    }
    return $null
}

function Invoke-AotItem($it) {
    try { $pt = $it.GetCurrentPattern([System.Windows.Automation.TogglePattern]::Pattern); $pt.Toggle(); return 'toggle' } catch {}
    try { $pt = $it.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern); $pt.Invoke(); return 'invoke' } catch {}
    return 'none'
}

function Get-AotState($it) {
    try { $pt = $it.GetCurrentPattern([System.Windows.Automation.TogglePattern]::Pattern); return $pt.Current.ToggleState.ToString() } catch { return 'n/a' }
}

function TopMostBit { [bool](([U32]::GetWindowLongW($h, -20) -band 0x8) -ne 0) }
function ExStyleHex { "0x{0:X}" -f [U32]::GetWindowLongW($h, -20) }

# 1. initial z-order must be normal (not topmost)
Check 'initial-topmost-off' (-not (TopMostBit)) (ExStyleHex)

# 2. right-click title bar -> menu opens; invoke "Always on Top"
Click-Client 324 14 'right'
$script:menuHwnd = [IntPtr]::Zero
$dumped = $false
WaitUntil {
    $script:menuHwnd = Find-MenuWindow
    if ($script:menuHwnd -ne [IntPtr]::Zero) { return $true }
    if (-not $dumped) {
        $dumped = $true
        Write-Host 'DUMP app windows:'
        foreach ($hw2 in [U32]::WindowsOf($w)) {
            $mr2 = New-Object U32+RECT
            [U32]::GetWindowRect($hw2, [ref]$mr2) | Out-Null
            Write-Host ("  hwnd=" + $hw2 + " class=" + [U32]::ClassName($hw2) + " rect=(" + $mr2.L + "," + $mr2.T + "," + ($mr2.R - $mr2.L) + "x" + ($mr2.B - $mr2.T) + ")")
        }
    }
    return $false
} 2500 | Out-Null
$it = Find-AotItemInMenu
$st = $(if ($it) { Get-AotState $it } else { 'none' })
Check 'menu-opens-on-titlebar' ($it -ne $null) ('state=' + $st)
if ($it) {
    Shot '1-titlebar-menu.png'
    $how = Invoke-AotItem $it
    Write-Output ("MENU invoked via " + $how)
}
Start-Sleep -Milliseconds 400
Assert-Alive 'after menu invoke'
Check 'toggle-on-topmost' (TopMostBit) (ExStyleHex)

# 3. reopen: item must show checked; toggle back off
Click-Client 324 14 'right'
$script:menuHwnd = [IntPtr]::Zero
WaitUntil { $script:menuHwnd = Find-MenuWindow; $script:menuHwnd -ne [IntPtr]::Zero } 2500 | Out-Null
$it = Find-AotItemInMenu
$st = $(if ($it) { Get-AotState $it } else { 'none' })
Check 'menu-checkstate-on' (($it -ne $null) -and ($st -eq 'On')) ('state=' + $st)
if ($it) { Invoke-AotItem $it | Out-Null }
Start-Sleep -Milliseconds 400
Assert-Alive 'after toggle off'
Check 'toggle-off-topmost' (-not (TopMostBit)) (ExStyleHex)

# 4. right-click inside web content must NOT open the title bar menu
Click-Client 324 300 'right'
Start-Sleep -Milliseconds 500
$script:menuHwnd = [IntPtr]::Zero
$found = WaitUntil { $script:menuHwnd = Find-MenuWindow; $script:menuHwnd -ne [IntPtr]::Zero } 1200
Check 'web-rightclick-no-titlebar-menu' (-not $found) (($(if ($found) { 'unexpected WinForms menu' } else { 'no WinForms menu popup' })))
Shot '2-web-context.png'
Guarded-Keys '{ESC}' | Out-Null
Start-Sleep -Milliseconds 200

# 5. navigate to local fullscreen test page via the url bar
Assert-Alive 'before navigation'
Click-Client 324 546 'left'
Start-Sleep -Milliseconds 250
$fstest = (Join-Path $PSScriptRoot 'verify-out\fstest.html') -replace '\\', '/'
$ok = Guarded-Keys ('file:///' + $fstest + '{ENTER}')
Start-Sleep -Milliseconds 3000
Assert-Alive 'after navigation'
Shot '3-fstest-loaded.png'

# 6. click page -> element fullscreen -> window floats AND keeps its bounds
$before = GetRect
Click-Client 324 300 'left'
$ok = WaitUntil { TopMostBit } 4000
Assert-Alive 'after fullscreen click'
Check 'fullscreen-floats-on-top' $ok (ExStyleHex)
$after = GetRect
Check 'fullscreen-stays-in-window' (([Math]::Abs($after.L - $before.L) -le 4) -and ([Math]::Abs($after.T - $before.T) -le 4) -and ([Math]::Abs(($after.R - $after.L) - ($before.R - $before.L)) -le 4) -and ([Math]::Abs(($after.B - $after.T) - ($before.B - $before.T)) -le 4)) ("before=(" + $before.L + "," + $before.T + "," + ($before.R - $before.L) + "x" + ($before.B - $before.T) + ") after=(" + $after.L + "," + $after.T + "," + ($after.R - $after.L) + "x" + ($after.B - $after.T) + ")")
Shot '4-fullscreen.png'
Guarded-Keys '{ESC}' | Out-Null
$ok = WaitUntil { -not (TopMostBit) } 4000
Check 'fullscreen-exit-restores' $ok (ExStyleHex)

# 7. maximize respects the taskbar
Click-Client 580 15 'left'
$ok = WaitUntil { $rr = GetRect; ($rr.R - $rr.L) -ge ($wa.Width - 4) } 5000
$rr = GetRect
Check 'maximize-in-workarea' ($ok -and ($rr.T -ge $wa.Top - 2) -and ($rr.B -le $wa.Bottom + 2)) ("win=(" + $rr.L + "," + $rr.T + "," + ($rr.R - $rr.L) + "x" + ($rr.B - $rr.T) + ") workarea=" + $wa.Width + "x" + $wa.Height)
Shot '5-maximized.png'
Click-Client 580 15 'left'
$ok = WaitUntil { $rr2 = GetRect; ($rr2.R - $rr2.L) -lt ($wa.Width - 4) } 5000
Check 'restore-after-max' $ok ('width=' + $(if ($ok) { 'restored' } else { 'still maximized' }))

# 8. close via close button
Click-Client 619 12 'left'
$ok = WaitUntil { $p.Refresh(); $p.HasExited } 5000
if (-not $ok) { Get-Process FloatCore3 -ErrorAction SilentlyContinue | Stop-Process -Force }
Check 'close-button-exits' $ok 'closeButton_Click -> Application.Exit'

Write-Output ("RESULT fails=" + $script:failCount)
exit $script:failCount

