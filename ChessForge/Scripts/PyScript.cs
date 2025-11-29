using Avalonia;
using Avalonia.Media;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ChessForge.Scripts
{
    public class PyScript : IScript
    {
        private ScriptRender render = new();
        private string path;

        private dynamic _sys;
        private dynamic _module;
        private dynamic _pychess;

        public PyScript(string path) {
            if (!PythonEngine.IsInitialized)
            {
                Runtime.PythonDLL = GetPythonDllPath();
                PythonEngine.Initialize();
                // 使用这个后可以不使用 PythonEngine.Shutdown() -> 因为这个有漏洞
                PythonEngine.BeginAllowThreads();

                Load(path);

            }
            
        }
        public void Load(string path)
        {
            this.path = path;

            using (Py.GIL())
            {
                _sys = Py.Import("sys");
                _sys.path.append(Path.GetDirectoryName(path));
                _module = Py.Import(Path.GetFileNameWithoutExtension(path));

                dynamic scope = Py.CreateScope();
                scope.Set("render", render.ToPython());
                _pychess = _module.LoadChess(render);
            }
        }

        public void Reload()
        {
            
        }

        public Action<DrawingContext, Rect>? Render()
        {
            using (Py.GIL())
            {
                _pychess.Render();
            }
            return render.DrawBoard;
        }
        /// <summary>
        /// 判读是否胜利, 1 表示胜利，0 表示未结束，-1 表示失败 (从上次Click走棋的玩家来看)
        /// </summary>
        /// <returns></returns>
        public int IsWin()
        {
            using (Py.GIL())
            {
                return _pychess.IsWin();
            }
        }

        public void Close()
        {
            if (PythonEngine.IsInitialized)
            {
                //PythonEngine.Shutdown();
            }

        }

        public void Click(double x, double y)
        {
            using (Py.GIL())
            {
                _pychess.Click( x, y);
            }
        }

        public void NewGame()
        {
            using (Py.GIL())
            {
                _pychess.NewGame();
            }
        }

        ~PyScript()
        {
            Close();
        }


        /// <summary>
        /// 自动查找 Python DLL/SO 路径，支持 Windows 和 Linux，使用控制台命令。
        /// </summary>
        /// <param name="pythonExecutable">Python 可执行文件名称，例如 "python" 或 "python3"（默认 "python3"）</param>
        /// <param name="requiredVersion">最低版本，例如 "3.8"（可选）</param>
        /// <returns>Python DLL/SO 的完整路径，或空字符串如果未找到</returns>
        private static string GetPythonDllPath(string pythonExecutable = "python", string requiredVersion = "")
        {
            // 步骤1: 检测操作系统
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return GetWindowsPythonDllPath(pythonExecutable, requiredVersion);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return GetLinuxPythonDllPath(pythonExecutable, requiredVersion);
            }
            else
            {
                throw new PlatformNotSupportedException("仅支持 Windows 和 Linux。");
            }
        }

        /// <summary>
        /// Windows 特定：使用命令行查找 Python DLL 路径。
        /// 假设 python 在 PATH 中。
        /// </summary>
        private static string GetWindowsPythonDllPath(string pythonExecutable, string requiredVersion = "")
        {
            // 步骤1: 获取 Python 版本
            string versionOutput = RunCommand(pythonExecutable, "--version");
            if (string.IsNullOrEmpty(versionOutput)) return "";

            // 解析版本，例如 "Python 3.12.0" -> "3.12"
            string[] parts = versionOutput.Split(' ');
            if (parts.Length < 2 || !parts[0].Equals("Python", StringComparison.OrdinalIgnoreCase)) return "";
            string pyVersion = parts[1].Trim();
            string majorMinor = pyVersion.Substring(0, pyVersion.LastIndexOf('.'));

            if (!string.IsNullOrEmpty(requiredVersion))
            {
                if (Version.Parse(majorMinor) < Version.Parse(requiredVersion)) return "";
            }

            // 步骤2: 获取 Python 安装路径 (sys.prefix)
            string prefixScript = "import sys; print(sys.prefix)";
            string prefix = RunCommand(pythonExecutable, "-c", $"\"{prefixScript}\"").Trim();
            if (string.IsNullOrEmpty(prefix)) return "";

            // 步骤3: 获取 DLL 名称 (sysconfig.get_config_var('DLL'))
            // Windows 上通常是 pythonXY.dll，其中 XY 是 major minor 无点
            string majorMinorNoDot = majorMinor.Replace(".", "");
            string dllName = $"python{majorMinorNoDot}.dll";

            // 构造 DLL 路径：通常在 prefix 下
            string pythonDllPath = System.IO.Path.Combine(prefix, dllName);

            // 检查是否存在，否则尝试在 prefix/DLLs
            if (System.IO.File.Exists(pythonDllPath)) return pythonDllPath;

            string dllsPath = System.IO.Path.Combine(prefix, "DLLs", dllName);
            if (System.IO.File.Exists(dllsPath)) return dllsPath;

            // 备选：使用 where 命令查找 python.exe，然后推导 DLL
            string exePath = RunCommand("where", pythonExecutable).Trim().Split('\n')[0];  // 取第一个
            if (!string.IsNullOrEmpty(exePath))
            {
                string installDir = System.IO.Path.GetDirectoryName(exePath);
                pythonDllPath = System.IO.Path.Combine(installDir, dllName);
                if (System.IO.File.Exists(pythonDllPath)) return pythonDllPath;
            }

            return "";
        }

        /// <summary>
        /// Linux 特定：使用命令行查找 Python 共享库路径。
        /// </summary>
        private static string GetLinuxPythonDllPath(string pythonExecutable, string requiredVersion = "")
        {
            // 步骤1: 获取 Python 版本
            string versionOutput = RunCommand(pythonExecutable, "--version");
            if (string.IsNullOrEmpty(versionOutput)) return "";

            string[] parts = versionOutput.Split(' ');
            if (parts.Length < 2 || !parts[0].Equals("Python", StringComparison.OrdinalIgnoreCase)) return "";
            string pyVersion = parts[1].Trim();
            string majorMinor = pyVersion.Substring(0, pyVersion.LastIndexOf('.'));

            if (!string.IsNullOrEmpty(requiredVersion))
            {
                if (Version.Parse(majorMinor) < Version.Parse(requiredVersion)) return "";
            }

            // 步骤2: 使用 sysconfig 获取 LIBDIR 和 LDLIBRARY
            string libDirScript = "import sysconfig; print(sysconfig.get_config_var('LIBDIR'))";
            string libDir = RunCommand(pythonExecutable, "-c", $"\"{libDirScript}\"").Trim();
            if (string.IsNullOrEmpty(libDir)) return "";

            string soNameScript = "import sysconfig; print(sysconfig.get_config_var('LDLIBRARY'))";
            string soName = RunCommand(pythonExecutable, "-c", $"\"{soNameScript}\"").Trim();
            if (string.IsNullOrEmpty(soName)) soName = $"libpython{majorMinor.Replace(".", "")}.so";

            string pythonSoPath = System.IO.Path.Combine(libDir, soName);

            if (System.IO.File.Exists(pythonSoPath)) return pythonSoPath;

            // 备选：使用 ldconfig 搜索
            string ldOutput = RunCommand("ldconfig", "-p");
            if (!string.IsNullOrEmpty(ldOutput))
            {
                var lines = ldOutput.Split('\n');
                foreach (var line in lines)
                {
                    if (line.Contains($"libpython{majorMinor}"))
                    {
                        // 格式如 "libpython3.10.so (libc6,x86-64) => /usr/lib/x86_64-linux-gnu/libpython3.10.so"
                        var pathPart = line.Split("=> ");
                        if (pathPart.Length > 1) return pathPart[1].Trim();
                    }
                }
            }

            return "";
        }

        /// <summary>
        /// 运行外部命令并返回输出。
        /// </summary>
        private static string RunCommand(string exe, params string[] args)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = exe,
                    Arguments = string.Join(" ", args),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    if (process == null) return "";

                    string output = process.StandardOutput.ReadToEnd().Trim();
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        string error = process.StandardError.ReadToEnd().Trim();
                        Console.WriteLine($"命令执行错误: {error}");
                        return "";
                    }

                    return output;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"运行命令异常: {ex.Message}");
                return "";
            }
        }
    }
}
