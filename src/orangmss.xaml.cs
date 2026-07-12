using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Diagnostics;
using System.Windows;
using System.Security.Principal;
namespace OrangMSS
{
    public partial class MainWindow : Window
    {
        private CancellationTokenSource? _cts;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void AppendLog(string text)
        {
            Dispatcher.Invoke(() =>
            {
                LogTextBox.AppendText(text + Environment.NewLine);
                LogTextBox.ScrollToEnd();
            });
        }
        private static bool IsAdministrator()
        {
            using WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _cts?.Cancel();
            CancelButton.IsEnabled = false;
        }
        private async void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdministrator())
            {
                var result = MessageBox.Show("This action requires Administrator privileges. Restart as Administrator?", "Elevation Required", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    var exeName = Environment.ProcessPath ?? string.Empty;
                    if (string.IsNullOrEmpty(exeName))
                    {
                        MessageBox.Show("Cannot elevate: unable to determine executable path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    ProcessStartInfo startInfo = new(exeName)
                    {
                        UseShellExecute = true,
                        Verb = "runas"
                    };
                    try
                    {
                        Process.Start(startInfo);
                        Application.Current.Shutdown();
                        return;
                    }
                    catch (Win32Exception)
                    {
                        MessageBox.Show("Elevation was cancelled.", "OrangMSS", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                return;
            }
            InstallButton.IsEnabled = false;
            UninstallButton.IsEnabled = false;
            CancelButton.IsEnabled = true;
            ProgressBar.Value = 0;
            _cts = new CancellationTokenSource();
            var token = _cts.Token;
            var dataDir = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OrangStudio", "OrangMSS");
            Directory.CreateDirectory(dataDir);
            var downloads = new (string url, string filename)[]
            {
                ("https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.XboxIdentityProvider_8wekyb3d8bbwe.xml","Microsoft.XboxIdentityProvider_8wekyb3d8bbwe.xml"),
                ("https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.XboxIdentityProvider_12.45.6001.0_neutral_~_8wekyb3d8bbwe.AppxBundle","Microsoft.XboxIdentityProvider_12.45.6001.0_neutral_~_8wekyb3d8bbwe.AppxBundle"),
                ("https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.WindowsStore_8wekyb3d8bbwe.xml","Microsoft.WindowsStore_8wekyb3d8bbwe.xml"),
                ("https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.WindowsStore_11809.1001.713.0_neutral_~_8wekyb3d8bbwe.AppxBundle","Microsoft.WindowsStore_11809.1001.713.0_neutral_~_8wekyb3d8bbwe.AppxBundle"),
                ("https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.VCLibs.140.00_14.0.26706.0_x86__8wekyb3d8bbwe.Appx","Microsoft.VCLibs.140.00_14.0.26706.0_x86__8wekyb3d8bbwe.Appx"),
                ("https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.VCLibs.140.00_14.0.26706.0_x64__8wekyb3d8bbwe.Appx","Microsoft.VCLibs.140.00_14.0.26706.0_x64__8wekyb3d8bbwe.Appx"),
                ("https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.StorePurchaseApp_8wekyb3d8bbwe.xml","Microsoft.StorePurchaseApp_8wekyb3d8bbwe.xml"),
                ("https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.StorePurchaseApp_11808.1001.413.0_neutral_~_8wekyb3d8bbwe.AppxBundle","Microsoft.StorePurchaseApp_11808.1001.413.0_neutral_~_8wekyb3d8bbwe.AppxBundle"),
                ("https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.NET.Native.Runtime.1.6_1.6.24903.0_x86__8wekyb3d8bbwe.Appx","Microsoft.NET.Native.Runtime.1.6_1.6.24903.0_x86__8wekyb3d8bbwe.Appx"),
                ("https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.NET.Native.Runtime.1.6_1.6.24903.0_x64__8wekyb3d8bbwe.Appx","Microsoft.NET.Native.Runtime.1.6_1.6.24903.0_x64__8wekyb3d8bbwe.Appx"),
                ("https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.NET.Native.Framework.1.6_1.6.24903.0_x86__8wekyb3d8bbwe.Appx","Microsoft.NET.Native.Framework.1.6_1.6.24903.0_x86__8wekyb3d8bbwe.Appx"),
                ("https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.NET.Native.Framework.1.6_1.6.24903.0_x64__8wekyb3d8bbwe.Appx","Microsoft.NET.Native.Framework.1.6_1.6.24903.0_x64__8wekyb3d8bbwe.Appx"),
                ("https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.DesktopAppInstaller_8wekyb3d8bbwe.xml","Microsoft.DesktopAppInstaller_8wekyb3d8bbwe.xml"),
                ("https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.DesktopAppInstaller_1.6.29000.1000_neutral_~_8wekyb3d8bbwe.AppxBundle","Microsoft.DesktopAppInstaller_1.6.29000.1000_neutral_~_8wekyb3d8bbwe.AppxBundle")
            };
            using var http = new HttpClient();
            int total = downloads.Length;
            try
            {
                for (int i = 0; i < downloads.Length; i++)
                {
                    token.ThrowIfCancellationRequested();
                    var (url, filename) = downloads[i];
                    var dest = Path.Join(dataDir, filename);
                    if (!File.Exists(dest) || new FileInfo(dest).Length == 0)
                    {
                        AppendLog($"Downloading {url} -> {dest}");
                        using var resp = await http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);
                        resp.EnsureSuccessStatusCode();
                        var contentLength = resp.Content.Headers.ContentLength ?? -1L;
                        using var contentStream = await resp.Content.ReadAsStreamAsync();
                        using var fs = File.Create(dest);
                        var buffer = new byte[81920];
                        long totalRead = 0;
                        int read;
                        while ((read = await contentStream.ReadAsync(buffer.AsMemory(0, buffer.Length), token)) > 0)
                        {
                            token.ThrowIfCancellationRequested();
                            await fs.WriteAsync(buffer.AsMemory(0, read), token);
                            totalRead += read;

                            if (contentLength > 0)
                            {
                                double fileProgress = (double)totalRead / contentLength;
                                double overall = ((double)i + fileProgress) / total * 30.0;
                                ProgressBar.Value = overall;
                            }
                        }
                    }
                    AppendLog($"Downloaded/Checked: {filename}");
                    ProgressBar.Value = ((double)(i + 1) / total) * 30.0;
                }
                AppendLog("Installation starting...");
                ProgressBar.Value = 35;
                await InstallPackagesAsync(dataDir, token);
                AppendLog("Installation Finished.");
                ProgressBar.Value = 100;
            }
            catch (OperationCanceledException)
            {
                AppendLog("Cancelled by user.");
            }
            catch (Exception ex) when (ex is HttpRequestException or IOException or UnauthorizedAccessException or InvalidOperationException or Win32Exception)
            {
                AppendLog("Error: " + ex.Message);
            }
            finally
            {
                InstallButton.IsEnabled = true;
                UninstallButton.IsEnabled = true;
                CancelButton.IsEnabled = false;
            }
        }
        private async void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdministrator())
            {
                var result = MessageBox.Show("This action requires Administrator privileges. Restart as Administrator?", "Elevation Required", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    var exeName = Environment.ProcessPath ?? string.Empty;
                    if (string.IsNullOrEmpty(exeName))
                    {
                        MessageBox.Show("Cannot elevate: unable to determine executable path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    ProcessStartInfo startInfo = new(exeName)
                    {
                        UseShellExecute = true,
                        Verb = "runas"
                    };
                    try
                    {
                        Process.Start(startInfo);
                        Application.Current.Shutdown();
                        return;
                    }
                    catch (Win32Exception)
                    {
                        MessageBox.Show("Elevation was cancelled.", "OrangMSS", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                return;
            }
            InstallButton.IsEnabled = false;
            UninstallButton.IsEnabled = false;
            CancelButton.IsEnabled = true;
            ProgressBar.Value = 0;
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                AppendLog("Uninstalling Microsoft Store and dependencies...");

                var packages = new[] {
                    "Microsoft.WindowsStore",
                    "Microsoft.StorePurchaseApp",
                    "Microsoft.DesktopAppInstaller",
                    "Microsoft.XboxIdentityProvider"
                };

                ProgressBar.Value = 10;
                int step = 80 / packages.Length;

                for (int i = 0; i < packages.Length; i++)
                {
                    token.ThrowIfCancellationRequested();
                    var pkg = packages[i];
                    AppendLog($"Removing {pkg}...");
                    await RunPowerShellAsync($"Get-AppxPackage -AllUsers *{pkg}* | ForEach-Object {{ Remove-AppxPackage -Package $_.PackageFullName -AllUsers -ErrorAction SilentlyContinue }}", token);
                    await RunPowerShellAsync($"Get-AppxPackage *{pkg}* | ForEach-Object {{ Remove-AppxPackage -Package $_.PackageFullName -ErrorAction SilentlyContinue }}", token);
                    await RunPowerShellAsync($"Get-AppxProvisionedPackage -Online | Where-Object DisplayName -like '*{pkg}*' | Remove-AppxProvisionedPackage -Online -ErrorAction SilentlyContinue | Out-Null", token);
                    ProgressBar.Value += step;
                }
                AppendLog("Uninstallation Finished.");
                ProgressBar.Value = 100;
            }
            catch (OperationCanceledException)
            {
                AppendLog("Cancelled by user.");
            }
            catch (Exception ex) when (ex is HttpRequestException or IOException or UnauthorizedAccessException or InvalidOperationException or Win32Exception)
            {
                AppendLog("Error: " + ex.Message);
            }
            finally
            {
                InstallButton.IsEnabled = true;
                UninstallButton.IsEnabled = true;
                CancelButton.IsEnabled = false;
            }
        }
        private async Task InstallPackagesAsync(string dataDir, CancellationToken token)
        {
            var arch = Environment.Is64BitOperatingSystem ? "x64" : "x86";
            var storeBundle = Directory.GetFiles(dataDir, "*WindowsStore*.AppxBundle").FirstOrDefault();
            var storeXml = Path.Join(dataDir, "Microsoft.WindowsStore_8wekyb3d8bbwe.xml");
            if (storeBundle == null || !File.Exists(storeXml))
            {
                throw new InvalidOperationException("Required Store files are missing.");
            }
            var fX64 = Directory.GetFiles(dataDir, "*Framework*").FirstOrDefault(f => f.Contains("x64", StringComparison.OrdinalIgnoreCase));
            var fX86 = Directory.GetFiles(dataDir, "*Framework*").FirstOrDefault(f => f.Contains("x86", StringComparison.OrdinalIgnoreCase));
            var rX64 = Directory.GetFiles(dataDir, "*Runtime*").FirstOrDefault(f => f.Contains("x64", StringComparison.OrdinalIgnoreCase));
            var rX86 = Directory.GetFiles(dataDir, "*Runtime*").FirstOrDefault(f => f.Contains("x86", StringComparison.OrdinalIgnoreCase));
            var vX64 = Directory.GetFiles(dataDir, "*VCLibs*").FirstOrDefault(f => f.Contains("x64", StringComparison.OrdinalIgnoreCase));
            var vX86 = Directory.GetFiles(dataDir, "*VCLibs*").FirstOrDefault(f => f.Contains("x86", StringComparison.OrdinalIgnoreCase));
            string?[] dependencies = arch == "x64"
                ? [vX64, vX86, fX64, fX86, rX64, rX86]
                : [vX86, fX86, rX86];
            if (dependencies.Any(d => d == null))
            {
                throw new InvalidOperationException("Some dependencies are missing.");
            }
            var nonNullDeps = dependencies.Select(d => d!).ToArray();
            var depString = string.Join("','", nonNullDeps);
            AppendLog("Installing Microsoft Store...");
            foreach(var dep in nonNullDeps)
                await RunPowerShellAsync($"Add-AppxPackage -Path '{dep}'", token);
            await RunPowerShellAsync($"Add-AppxProvisionedPackage -Online -PackagePath '{storeBundle}' -DependencyPackagePath '{depString}' -LicensePath '{storeXml}'", token);
            await RunPowerShellAsync($"Add-AppxPackage -Path '{storeBundle}'", token);
            ProgressBar.Value = 55;
            var pbApp = Directory.GetFiles(dataDir, "*StorePurchaseApp*.AppxBundle").FirstOrDefault();
            var pbXml = Path.Join(dataDir, "Microsoft.StorePurchaseApp_8wekyb3d8bbwe.xml");
            if (pbApp != null && File.Exists(pbXml))
            {
                AppendLog("Installing Store Purchase App...");
                await RunPowerShellAsync($"Add-AppxProvisionedPackage -Online -PackagePath '{pbApp}' -DependencyPackagePath '{depString}' -LicensePath '{pbXml}'", token);
                await RunPowerShellAsync($"Add-AppxPackage -Path '{pbApp}'", token);
            }
            ProgressBar.Value = 75;
            var installerApp = Directory.GetFiles(dataDir, "*DesktopAppInstaller*.AppxBundle").FirstOrDefault();
            var installerXml = Path.Join(dataDir, "Microsoft.DesktopAppInstaller_8wekyb3d8bbwe.xml");
            if (installerApp != null && File.Exists(installerXml))
            {
                AppendLog("Installing App Installer...");
                var instDeps = arch == "x64" ? [vX64!, vX86!] : new[] { vX86! };
                await RunPowerShellAsync($"Add-AppxProvisionedPackage -Online -PackagePath '{installerApp}' -DependencyPackagePath '{string.Join("','", instDeps)}' -LicensePath '{installerXml}'", token);
                await RunPowerShellAsync($"Add-AppxPackage -Path '{installerApp}'", token);
            }
            ProgressBar.Value = 90;
            var xboxApp = Directory.GetFiles(dataDir, "*XboxIdentityProvider*.AppxBundle").FirstOrDefault();
            var xboxXml = Path.Join(dataDir, "Microsoft.XboxIdentityProvider_8wekyb3d8bbwe.xml");
            if (xboxApp != null && File.Exists(xboxXml))
            {
                AppendLog("Installing Xbox Identity Provider...");
                await RunPowerShellAsync($"Add-AppxProvisionedPackage -Online -PackagePath '{xboxApp}' -DependencyPackagePath '{depString}' -LicensePath '{xboxXml}'", token);
                await RunPowerShellAsync($"Add-AppxPackage -Path '{xboxApp}'", token);
            }
        }
        private async Task RunPowerShellAsync(string script, CancellationToken token)
        {
            var tcs = new TaskCompletionSource<bool>();
            using var proc = new Process();
            proc.StartInfo.FileName = "powershell.exe";
            proc.StartInfo.Arguments = $"-NoProfile -NonInteractive -ExecutionPolicy Bypass -Command \"{script}\"";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.CreateNoWindow = true;
            proc.EnableRaisingEvents = true;
            proc.OutputDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) AppendLog(e.Data); };
            proc.ErrorDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) AppendLog("ERR: " + e.Data); };
            proc.Exited += (s, e) => tcs.TrySetResult(true);
            using var reg = token.Register(() =>
            {
                try
                {
                    if (!proc.HasExited) proc.Kill();
                }
                catch (Exception ex) when (ex is InvalidOperationException or Win32Exception)
                {
                    AppendLog("Could not stop PowerShell: " + ex.Message);
                }
                tcs.TrySetCanceled();
            });
            proc.Start();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();
            await tcs.Task;
        }
    }
}