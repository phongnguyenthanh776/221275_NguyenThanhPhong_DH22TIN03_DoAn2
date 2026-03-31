using HealthManagement.Models;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Text.Json;

namespace HealthManagement.Services
{
    /// <summary>
    /// Interface cho AI Service - hỗ trợ dự đoán 4 loại bệnh
    /// </summary>
    public interface IAIService
    {
        Task<PredictionResponse> PredictHeartDiseaseAsync(HeartDiseaseRequest request);
        Task<PredictionResponse> PredictDiabetesAsync(DiabetesRequest request);
        Task<PredictionResponse> PredictHypertensionAsync(HypertensionRequest request);
        Task<PredictionResponse> PredictStrokeAsync(StrokeRequest request);
        Task<PredictionResponse> PredictAsync(DiseaseType diseaseType, Dictionary<string, object> data);
        Task<PredictionResponse> PredictKidneyStoneImageAsync(byte[] imageBytes, string fileName);
        Task<PredictionResponse> PredictPneumoniaImageAsync(byte[] imageBytes, string fileName);
    }

    /// <summary>
    /// AI Service Implementation - gọi Flask API với trained models
    /// </summary>
    public class AIService : IAIService
    {
        private static readonly SemaphoreSlim FlaskStartupLock = new(1, 1);
        private static DateTime _lastFlaskStartupAttemptUtc = DateTime.MinValue;
        private static Process? _flaskProcess;
        private static readonly object FlaskDiagnosticsLock = new();
        private static string? _lastFlaskStartupFailure;

        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<AIService> _logger;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AIService(
            IConfiguration configuration,
            HttpClient httpClient,
            ILogger<AIService> logger,
            IWebHostEnvironment hostEnvironment)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        // ============================================================
        // Heart Disease Prediction
        // ============================================================
        public async Task<PredictionResponse> PredictHeartDiseaseAsync(HeartDiseaseRequest request)
        {
            _logger.LogInformation("🫀 Predicting Heart Disease...");
            
            var data = new Dictionary<string, object>
            {
                { "age", request.Age },
                { "sex", request.Sex },
                { "chestpaintype", request.ChestPainType ?? 0 },
                { "restingbp", request.RestingBP ?? 0 },
                { "cholesterol", request.Cholesterol ?? 0 },
                { "fastingbs", request.FastingBS ?? 0 },
                { "maxhr", request.MaxHR ?? 0 },
                { "exerciseangina", request.ExerciseAngina ?? 0 }
            };
            
            return await PredictAsync(DiseaseType.HeartDisease, data);
        }

        // ============================================================
        // Diabetes Prediction
        // ============================================================
        public async Task<PredictionResponse> PredictDiabetesAsync(DiabetesRequest request)
        {
            _logger.LogInformation("🩺 Predicting Diabetes...");
            
            var data = new Dictionary<string, object>
            {
                { "pregnancies", request.Pregnancies ?? 0 },
                { "glucose", request.Glucose ?? 0 },
                { "bloodpressure", request.BloodPressure ?? 0 },
                { "skinthickness", request.SkinThickness ?? 0 },
                { "insulin", request.Insulin ?? 0 },
                { "bmi", request.BMI ?? 0 },
                { "diabetespedigreefunction", request.DiabetesPedigreeFunction ?? 0 },
                { "age", request.Age ?? 0 }
            };
            
            return await PredictAsync(DiseaseType.Diabetes, data);
        }

        // ============================================================
        // Hypertension Prediction
        // ============================================================
        public async Task<PredictionResponse> PredictHypertensionAsync(HypertensionRequest request)
        {
            _logger.LogInformation("🔴 Predicting Hypertension...");
            
            var data = new Dictionary<string, object>
            {
                { "age", request.Age ?? 0 },
                { "gender", request.Gender ?? 0 },
                { "bmi", request.BMI ?? 0 },
                { "cholesterol", request.Cholesterol ?? 0 },
                { "systolicbp", request.SystolicBP ?? 0 },
                { "diastolicbp", request.DiastolicBP ?? 0 },
                { "heartrate", request.HeartRate ?? 0 },
                { "smoking", request.Smoking ?? 0 },
                { "alcohol", request.Alcohol ?? 0 },
                { "physicalactivity", request.PhysicalActivity ?? 0 }
            };
            
            return await PredictAsync(DiseaseType.Hypertension, data);
        }

        // ============================================================
        // Stroke Prediction
        // ============================================================
        public async Task<PredictionResponse> PredictStrokeAsync(StrokeRequest request)
        {
            _logger.LogInformation("🧠 Predicting Stroke...");
            
            var data = new Dictionary<string, object>
            {
                { "age", request.Age ?? 0 },
                { "gender", request.Gender ?? 0 },
                { "hypertension", request.Hypertension ?? 0 },
                { "heartdisease", request.HeartDisease ?? 0 },
                { "smoking", request.Smoking ?? 0 },
                { "bmi", request.BMI ?? 0 },
                { "avgbloodpressure", request.AvgBloodPressure ?? 0 },
                { "glucose", request.Glucose ?? 0 }
            };
            
            return await PredictAsync(DiseaseType.Stroke, data);
        }

        // ============================================================
        // IMAGE PREDICTION
        // ============================================================
        public async Task<PredictionResponse> PredictKidneyStoneImageAsync(byte[] imageBytes, string fileName)
        {
            _logger.LogInformation("🪨 Predicting Kidney Stone from image...");
            return await PredictImageAsync(DiseaseType.KidneyStone, imageBytes, fileName);
        }

        public async Task<PredictionResponse> PredictPneumoniaImageAsync(byte[] imageBytes, string fileName)
        {
            _logger.LogInformation("🫁 Predicting Pneumonia from image...");
            return await PredictImageAsync(DiseaseType.Pneumonia, imageBytes, fileName);
        }

        private async Task<PredictionResponse> PredictImageAsync(DiseaseType diseaseType, byte[] imageBytes, string fileName)
        {
            try
            {
                bool isAIEnabled = _configuration.GetValue<bool>("AISettings:IsEnabled");

                if (isAIEnabled)
                {
                    return await CallFlaskImageApiAsync(diseaseType, imageBytes, fileName);
                }

                _logger.LogWarning("⚠️ AI Service is disabled");
                return GenerateFallbackResponse(diseaseType);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Image prediction error: {ex.Message}");
                return GenerateFallbackResponse(diseaseType, isError: true);
            }
        }

        // ============================================================
        // Generic Prediction - gọi Flask API
        // ============================================================
        public async Task<PredictionResponse> PredictAsync(DiseaseType diseaseType, Dictionary<string, object> data)
        {
            try
            {
                bool isAIEnabled = _configuration.GetValue<bool>("AISettings:IsEnabled");
                
                if (isAIEnabled)
                {
                    return await CallFlaskAPIAsync(diseaseType, data);
                }

                _logger.LogWarning("⚠️ AI Service is disabled");
                return GenerateFallbackResponse(diseaseType);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Prediction error: {ex.Message}");
                return GenerateFallbackResponse(diseaseType, isError:true);
            }
        }

        // ============================================================
        // Flask API Call
        // ============================================================
        private async Task<PredictionResponse> CallFlaskAPIAsync(DiseaseType diseaseType, Dictionary<string, object> data)
        {
            try
            {
                var apiUrl = _configuration["AISettings:ApiUrl"];

                if (string.IsNullOrWhiteSpace(apiUrl))
                {
                    _logger.LogError("❌ Missing configuration: AISettings:ApiUrl");
                    return GenerateFallbackResponse(diseaseType, isError: true);
                }
                
                _logger.LogInformation($"🔗 Calling Flask API: {apiUrl}");
                _logger.LogInformation($"   Disease: {diseaseType}");
                
                // Map diseaseType to Flask API format
                var diseaseTypeStr = MapDiseaseTypeToApiFormat(diseaseType);
                
                // Tạo request payload
                var requestPayload = new
                {
                    disease_type = diseaseTypeStr,
                    Data = data
                };
                
                // Gọi API (nếu Flask chưa chạy thì tự khởi động và thử lại 1 lần)
                var response = await PostToFlaskWithRetryAsync(apiUrl, requestPayload);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<PredictionResponse>();
                    
                    if (result != null)
                    {
                        _logger.LogInformation($"✅ Prediction result: {result.Result} ({result.RiskLevel}%)");
                        return result;
                    }
                }
                else
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("⚠️ API returned status: {StatusCode}, Body: {Body}", response.StatusCode, responseBody);
                }
                
                return GenerateFallbackResponse(diseaseType, isError:true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Flask API error: {ex.Message}");
                return GenerateFallbackResponse(diseaseType, isError:true);
            }
        }

        private async Task<PredictionResponse> CallFlaskImageApiAsync(DiseaseType diseaseType, byte[] imageBytes, string fileName)
        {
            try
            {
                var imageApiUrl = ResolveImageApiUrl();

                if (string.IsNullOrWhiteSpace(imageApiUrl))
                {
                    _logger.LogError("❌ Missing image API url configuration");
                    return GenerateFallbackResponse(diseaseType, isError: true);
                }

                var diseaseTypeStr = MapDiseaseTypeToApiFormat(diseaseType);
                _logger.LogInformation("🔗 Calling Flask Image API: {ApiUrl}", imageApiUrl);
                _logger.LogInformation("   Disease: {DiseaseType}", diseaseTypeStr);

                HttpResponseMessage response = await PostMultipartToFlaskWithRetryAsync(imageApiUrl, () =>
                {
                    var content = new MultipartFormDataContent();
                    content.Add(new StringContent(diseaseTypeStr), "disease_type");

                    var bytesContent = new ByteArrayContent(imageBytes);
                    bytesContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                    content.Add(bytesContent, "file", string.IsNullOrWhiteSpace(fileName) ? "upload.jpg" : fileName);

                    return content;
                });

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<PredictionResponse>();
                    if (result != null)
                    {
                        _logger.LogInformation("✅ Image prediction result: {Result} ({RiskLevel}%)", result.Result, result.RiskLevel);
                        return result;
                    }
                }
                else
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("⚠️ Image API returned status: {StatusCode}, Body: {Body}", response.StatusCode, responseBody);
                }

                return GenerateFallbackResponse(diseaseType, isError: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("❌ Flask Image API error: {Message}", ex.Message);
                return GenerateFallbackResponse(diseaseType, isError: true);
            }
        }

        private async Task<HttpResponseMessage> PostToFlaskWithRetryAsync(string apiUrl, object requestPayload)
        {
            try
            {
                return await _httpClient.PostAsJsonAsync(apiUrl, requestPayload);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "⚠️ Flask API unreachable at {ApiUrl}", apiUrl);

                var started = await TryStartFlaskApiAsync(apiUrl);
                if (!started)
                {
                    var diagnostic = GetLastFlaskStartupFailure();
                    if (!string.IsNullOrWhiteSpace(diagnostic))
                    {
                        _logger.LogWarning("⚠️ Flask auto-start failed: {Diagnostic}", diagnostic);
                    }
                    throw;
                }

                _logger.LogInformation("✅ Flask API started. Retrying prediction request...");

                const int maxRetryAttempts = 3;
                for (var attempt = 1; attempt <= maxRetryAttempts; attempt++)
                {
                    try
                    {
                        return await _httpClient.PostAsJsonAsync(apiUrl, requestPayload);
                    }
                    catch (HttpRequestException retryEx) when (attempt < maxRetryAttempts)
                    {
                        _logger.LogWarning(retryEx, "⚠️ Retry {Attempt}/{Max} failed while waiting for Flask warm-up", attempt, maxRetryAttempts);
                        await Task.Delay(1000);
                    }
                }

                return await _httpClient.PostAsJsonAsync(apiUrl, requestPayload);
            }
        }

        private async Task<HttpResponseMessage> PostMultipartToFlaskWithRetryAsync(string apiUrl, Func<MultipartFormDataContent> contentFactory)
        {
            try
            {
                using var firstContent = contentFactory();
                return await _httpClient.PostAsync(apiUrl, firstContent);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "⚠️ Flask Image API unreachable at {ApiUrl}", apiUrl);

                var started = await TryStartFlaskApiAsync(apiUrl);
                if (!started)
                {
                    var diagnostic = GetLastFlaskStartupFailure();
                    if (!string.IsNullOrWhiteSpace(diagnostic))
                    {
                        _logger.LogWarning("⚠️ Flask image auto-start failed: {Diagnostic}", diagnostic);
                    }
                    throw;
                }

                _logger.LogInformation("✅ Flask API started. Retrying image prediction request...");

                const int maxRetryAttempts = 3;
                for (var attempt = 1; attempt <= maxRetryAttempts; attempt++)
                {
                    try
                    {
                        using var retryContent = contentFactory();
                        return await _httpClient.PostAsync(apiUrl, retryContent);
                    }
                    catch (HttpRequestException retryEx) when (attempt < maxRetryAttempts)
                    {
                        _logger.LogWarning(retryEx, "⚠️ Retry {Attempt}/{Max} failed while waiting for Flask warm-up", attempt, maxRetryAttempts);
                        await Task.Delay(1000);
                    }
                }

                using var finalContent = contentFactory();
                return await _httpClient.PostAsync(apiUrl, finalContent);
            }
        }

        private async Task<bool> TryStartFlaskApiAsync(string apiUrl)
        {
            var autoStartEnabled = _configuration.GetValue<bool>("AISettings:AutoStartIfUnavailable");
            if (!autoStartEnabled)
            {
                _logger.LogWarning("⚠️ Auto-start is disabled (AISettings:AutoStartIfUnavailable=false)");
                return false;
            }

            var healthUrl = BuildHealthUrl(apiUrl);
            if (await IsFlaskHealthyAsync(healthUrl))
            {
                ClearLastFlaskStartupFailure();
                return true;
            }

            await FlaskStartupLock.WaitAsync();
            try
            {
                if (await IsFlaskHealthyAsync(healthUrl))
                {
                    ClearLastFlaskStartupFailure();
                    return true;
                }

                if (DateTime.UtcNow - _lastFlaskStartupAttemptUtc < TimeSpan.FromSeconds(10))
                {
                    _logger.LogInformation("ℹ️ Flask startup was recently attempted. Skipping duplicate start.");
                }
                else
                {
                    _lastFlaskStartupAttemptUtc = DateTime.UtcNow;

                    var flaskScriptPath = ResolveFlaskScriptPath();
                    if (!File.Exists(flaskScriptPath))
                    {
                        _logger.LogWarning("⚠️ Flask script not found: {FlaskScriptPath}", flaskScriptPath);
                        SetLastFlaskStartupFailure($"Khong tim thay file flask_api.py tai: {flaskScriptPath}");
                        return false;
                    }

                    var pythonExecutable = ResolvePythonExecutable();

                    // Kill any stale process occupying the Flask port before starting fresh
                    KillProcessOnPort(apiUrl);

                    _flaskProcess = StartFlaskProcess(pythonExecutable, flaskScriptPath);
                    if (_flaskProcess == null)
                    {
                        foreach (var fallbackExecutable in GetPythonStartFallbacks(pythonExecutable))
                        {
                            _flaskProcess = StartFlaskProcess(fallbackExecutable, flaskScriptPath);
                            if (_flaskProcess != null)
                            {
                                break;
                            }
                        }
                    }

                    if (_flaskProcess == null)
                    {
                        _logger.LogWarning("⚠️ Failed to start Flask API process with all Python candidates");
                        SetLastFlaskStartupFailure("Khong the khoi dong Python de chay Flask API. Kiem tra Python/venv va quyen truy cap file.");
                        return false;
                    }

                    _flaskProcess.OutputDataReceived += (_, e) =>
                    {
                        if (!string.IsNullOrWhiteSpace(e.Data))
                        {
                            _logger.LogInformation("[Flask] {Line}", e.Data);
                        }
                    };
                    _flaskProcess.ErrorDataReceived += (_, e) =>
                    {
                        if (!string.IsNullOrWhiteSpace(e.Data))
                        {
                            _logger.LogWarning("[Flask][ERR] {Line}", e.Data);
                        }
                    };

                    _flaskProcess.BeginOutputReadLine();
                    _flaskProcess.BeginErrorReadLine();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to auto-start Flask API");
                SetLastFlaskStartupFailure($"Loi khi khoi dong Flask API: {ex.Message}");
                return false;
            }
            finally
            {
                FlaskStartupLock.Release();
            }

            var startupWaitSeconds = Math.Max(10, _configuration.GetValue<int>("AISettings:StartupWaitSeconds", 60));
            var timeoutAt = DateTime.UtcNow.AddSeconds(startupWaitSeconds);

            while (DateTime.UtcNow < timeoutAt)
            {
                if (await IsFlaskHealthyAsync(healthUrl))
                {
                    ClearLastFlaskStartupFailure();
                    return true;
                }

                await Task.Delay(500);
            }

            _logger.LogWarning("⚠️ Flask API did not become healthy after {Seconds}s", startupWaitSeconds);
            if (_flaskProcess?.HasExited == true)
            {
                SetLastFlaskStartupFailure($"Flask da thoat som voi ma loi {_flaskProcess.ExitCode}. Kiem tra dependency Python trong AIModel.");
            }
            else
            {
                SetLastFlaskStartupFailure($"Flask khong san sang sau {startupWaitSeconds}s. Thu tang AISettings:StartupWaitSeconds.");
            }
            return false;
        }

        private async Task<bool> IsFlaskHealthyAsync(string healthUrl)
        {
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                using var response = await _httpClient.GetAsync(healthUrl, cts.Token);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private Process? StartFlaskProcess(string pythonExecutable, string flaskScriptPath)
        {
            try
            {
                _logger.LogInformation("🚀 Starting Flask API using {PythonExecutable}", pythonExecutable);

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = pythonExecutable,
                    Arguments = BuildFlaskStartArguments(pythonExecutable, flaskScriptPath),
                    WorkingDirectory = Path.GetDirectoryName(flaskScriptPath)!,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                return Process.Start(processStartInfo);
            }
            catch (Win32Exception win32Ex)
            {
                _logger.LogWarning(win32Ex, "⚠️ Failed to start Flask API with {PythonExecutable}. Win32 error code: {ErrorCode}", pythonExecutable, win32Ex.NativeErrorCode);
                SetLastFlaskStartupFailure($"Khong the chay {pythonExecutable}. Win32={win32Ex.NativeErrorCode}.");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ Failed to start Flask API with {PythonExecutable}", pythonExecutable);
                SetLastFlaskStartupFailure($"Khong the chay {pythonExecutable}: {ex.Message}");
                return null;
            }
        }

        private static string BuildFlaskStartArguments(string pythonExecutable, string flaskScriptPath)
        {
            var executableName = Path.GetFileNameWithoutExtension(pythonExecutable);
            if (string.Equals(executableName, "py", StringComparison.OrdinalIgnoreCase))
            {
                return $"-3 -X utf8 \"{flaskScriptPath}\"";
            }

            return $"-X utf8 \"{flaskScriptPath}\"";
        }

        private IEnumerable<string> GetPythonStartFallbacks(string primaryExecutable)
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                primaryExecutable
            };

            foreach (var candidate in GetPythonExecutableCandidates())
            {
                if (seen.Add(candidate))
                {
                    yield return candidate;
                }
            }

            foreach (var fallback in new[] { "py", "python" })
            {
                if (seen.Add(fallback))
                {
                    yield return fallback;
                }
            }
        }

        private static void SetLastFlaskStartupFailure(string message)
        {
            lock (FlaskDiagnosticsLock)
            {
                _lastFlaskStartupFailure = message;
            }
        }

        private static void ClearLastFlaskStartupFailure()
        {
            lock (FlaskDiagnosticsLock)
            {
                _lastFlaskStartupFailure = null;
            }
        }

        private static string? GetLastFlaskStartupFailure()
        {
            lock (FlaskDiagnosticsLock)
            {
                return _lastFlaskStartupFailure;
            }
        }

        private static string BuildHealthUrl(string apiUrl)
        {
            if (Uri.TryCreate(apiUrl, UriKind.Absolute, out var uri))
            {
                return $"{uri.Scheme}://{uri.Host}:{uri.Port}/health";
            }

            return "http://localhost:5000/health";
        }

        private void KillProcessOnPort(string apiUrl)
        {
            int port = 5000;
            if (Uri.TryCreate(apiUrl, UriKind.Absolute, out var uri))
            {
                port = uri.Port;
            }

            try
            {
                // netstat -ano lists: proto  local  remote  state  pid
                var netstat = Process.Start(new ProcessStartInfo
                {
                    FileName = "netstat",
                    Arguments = "-ano",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                });

                if (netstat == null) return;

                var output = netstat.StandardOutput.ReadToEnd();
                netstat.WaitForExit();

                // Look for LISTENING lines on the target port
                foreach (var line in output.Split('\n'))
                {
                    var trimmed = line.Trim();
                    // Match lines like: TCP  0.0.0.0:5000  0.0.0.0:0  LISTENING  1234
                    if (!trimmed.StartsWith("TCP", StringComparison.OrdinalIgnoreCase)) continue;
                    if (!trimmed.Contains($":{port} ", StringComparison.Ordinal) &&
                        !trimmed.Contains($":{port}\t", StringComparison.Ordinal) &&
                        !trimmed.EndsWith($":{port}", StringComparison.Ordinal)) continue;

                    var parts = trimmed.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    // parts: [TCP, local_addr, remote_addr, state, PID] (LISTENING lines have 5 columns)
                    if (parts.Length < 5) continue;
                    if (!parts[3].Equals("LISTENING", StringComparison.OrdinalIgnoreCase)) continue;

                    if (int.TryParse(parts[4], out var pid) && pid > 0)
                    {
                        try
                        {
                            var stale = System.Diagnostics.Process.GetProcessById(pid);
                            _logger.LogWarning("⚠️ Killing stale process PID={Pid} ({Name}) on port {Port}", pid, stale.ProcessName, port);
                            stale.Kill(entireProcessTree: true);
                            stale.WaitForExit(2000);
                        }
                        catch (ArgumentException)
                        {
                            // Process already gone – fine
                        }
                        catch (Exception killEx)
                        {
                            _logger.LogWarning(killEx, "⚠️ Could not kill PID={Pid}", pid);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ KillProcessOnPort failed (non-fatal)");
            }
        }

        private string ResolveFlaskScriptPath()
        {
            var configuredPath = _configuration["AISettings:ScriptPath"];
            var scriptPath = string.IsNullOrWhiteSpace(configuredPath)
                ? Path.Combine("..", "AIModel", "flask_api.py")
                : configuredPath;

            if (Path.IsPathRooted(scriptPath))
            {
                return scriptPath;
            }

            foreach (var root in GetPathResolutionRoots())
            {
                var resolvedPath = Path.GetFullPath(Path.Combine(root, scriptPath));
                if (File.Exists(resolvedPath))
                {
                    return resolvedPath;
                }
            }

            return Path.GetFullPath(Path.Combine(_hostEnvironment.ContentRootPath, scriptPath));
        }

        private string ResolveImageApiUrl()
        {
            var configured = _configuration["AISettings:ImageApiUrl"];
            if (!string.IsNullOrWhiteSpace(configured))
            {
                return configured;
            }

            var apiUrl = _configuration["AISettings:ApiUrl"];
            if (string.IsNullOrWhiteSpace(apiUrl))
            {
                return "http://localhost:5000/predict-image";
            }

            if (apiUrl.EndsWith("/predict", StringComparison.OrdinalIgnoreCase))
            {
                return apiUrl[..^"/predict".Length] + "/predict-image";
            }

            return apiUrl.TrimEnd('/') + "/predict-image";
        }

        private static string MapDiseaseTypeToApiFormat(DiseaseType diseaseType)
        {
            return diseaseType switch
            {
                DiseaseType.HeartDisease => "heart_disease",
                DiseaseType.Diabetes => "diabetes",
                DiseaseType.Hypertension => "hypertension",
                DiseaseType.Stroke => "stroke",
                DiseaseType.KidneyStone => "kidney_stone_image",
                DiseaseType.Pneumonia => "pneumonia_image",
                _ => "heart_disease"
            };
        }

        private string ResolvePythonExecutable()
        {
            foreach (var candidate in GetPythonExecutableCandidates())
            {
                if (IsPythonExecutableUsable(candidate))
                {
                    return candidate;
                }

                if (TryRepairMovedVirtualEnvironment(candidate) && IsPythonExecutableUsable(candidate))
                {
                    _logger.LogInformation("✅ Repaired venv and selected Python executable: {PythonExecutable}", candidate);
                    return candidate;
                }
            }

            _logger.LogWarning("⚠️ Could not find a runnable Python executable. Falling back to `py` launcher.");
            return "py";
        }

        private IEnumerable<string> GetPythonExecutableCandidates()
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var roots = GetPathResolutionRoots().ToArray();

            var configured = _configuration["AISettings:PythonExecutable"];
            if (!string.IsNullOrWhiteSpace(configured))
            {
                if (Path.IsPathRooted(configured))
                {
                    foreach (var item in AddCandidate(configured)) yield return item;
                }
                else if (configured.Contains(Path.DirectorySeparatorChar) || configured.Contains(Path.AltDirectorySeparatorChar))
                {
                    foreach (var root in roots)
                    {
                        var resolvedConfigured = Path.GetFullPath(Path.Combine(root, configured));
                        foreach (var item in AddCandidate(resolvedConfigured)) yield return item;
                    }
                }
                else
                {
                    foreach (var item in AddCandidate(configured)) yield return item;
                }
            }

            foreach (var root in roots)
            {
                var defaults = new[]
                {
                    Path.GetFullPath(Path.Combine(root, "..", ".venv", "Scripts", "python.exe")),
                    Path.GetFullPath(Path.Combine(root, ".venv", "Scripts", "python.exe")),
                    Path.GetFullPath(Path.Combine(root, "..", "AIModel", "venv", "Scripts", "python.exe")),
                    Path.GetFullPath(Path.Combine(root, "..", "AIModel", ".venv", "Scripts", "python.exe"))
                };

                foreach (var candidate in defaults)
                {
                    foreach (var item in AddCandidate(candidate)) yield return item;
                }
            }

            var envPython = Environment.GetEnvironmentVariable("PYTHON");
            if (!string.IsNullOrWhiteSpace(envPython))
            {
                foreach (var item in AddCandidate(envPython)) yield return item;
            }

            foreach (var fallback in new[] { "py", "python" })
            {
                foreach (var item in AddCandidate(fallback)) yield return item;
            }

            IEnumerable<string> AddCandidate(string? value)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    yield break;
                }

                if (seen.Add(value))
                {
                    yield return value;
                }
            }
        }

        private IEnumerable<string> GetPathResolutionRoots()
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var candidate in new[] { _hostEnvironment.ContentRootPath, Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
            {
                if (string.IsNullOrWhiteSpace(candidate))
                {
                    continue;
                }

                var fullPath = Path.GetFullPath(candidate);
                if (seen.Add(fullPath))
                {
                    yield return fullPath;
                }
            }
        }

        private bool IsPythonExecutableUsable(string executablePath)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = executablePath,
                    Arguments = "--version",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using var process = Process.Start(startInfo);
                if (process == null)
                {
                    return false;
                }

                if (!process.WaitForExit(3000))
                {
                    try
                    {
                        process.Kill(entireProcessTree: true);
                    }
                    catch
                    {
                        // Ignore kill failures for probe process.
                    }
                    return false;
                }

                return process.ExitCode == 0;
            }
            catch (Win32Exception)
            {
                return false;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }

        private bool TryRepairMovedVirtualEnvironment(string pythonExecutablePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(pythonExecutablePath) || !Path.IsPathRooted(pythonExecutablePath))
                {
                    return false;
                }

                if (!pythonExecutablePath.EndsWith($"{Path.DirectorySeparatorChar}Scripts{Path.DirectorySeparatorChar}python.exe", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                var scriptsDirectory = Path.GetDirectoryName(pythonExecutablePath);
                if (string.IsNullOrWhiteSpace(scriptsDirectory))
                {
                    return false;
                }

                var venvDirectory = Directory.GetParent(scriptsDirectory)?.FullName;
                if (string.IsNullOrWhiteSpace(venvDirectory))
                {
                    return false;
                }

                var pyvenvConfigPath = Path.Combine(venvDirectory, "pyvenv.cfg");
                if (!File.Exists(pyvenvConfigPath))
                {
                    return false;
                }

                var homePython = ReadHomePythonFromPyvenvConfig(pyvenvConfigPath);
                if (string.IsNullOrWhiteSpace(homePython) || !File.Exists(homePython))
                {
                    _logger.LogWarning("⚠️ Cannot repair venv because base interpreter is missing: {BasePython}", homePython);
                    return false;
                }

                if (!IsPythonExecutableUsable(homePython))
                {
                    _logger.LogWarning("⚠️ Base interpreter from pyvenv.cfg is not runnable: {BasePython}", homePython);
                    return false;
                }

                _logger.LogWarning("⚠️ Detected moved/broken venv. Attempting repair: {VenvDirectory}", venvDirectory);

                var repairStartInfo = new ProcessStartInfo
                {
                    FileName = homePython,
                    Arguments = $"-m venv --upgrade \"{venvDirectory}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using var repairProcess = Process.Start(repairStartInfo);
                if (repairProcess == null)
                {
                    return false;
                }

                if (!repairProcess.WaitForExit(90000))
                {
                    try
                    {
                        repairProcess.Kill(entireProcessTree: true);
                    }
                    catch
                    {
                        // Ignore kill failures for repair process.
                    }

                    _logger.LogWarning("⚠️ Timed out while repairing venv at {VenvDirectory}", venvDirectory);
                    return false;
                }

                if (repairProcess.ExitCode != 0)
                {
                    var errorOutput = repairProcess.StandardError.ReadToEnd();
                    _logger.LogWarning("⚠️ venv repair failed with exit code {ExitCode}. Error: {Error}", repairProcess.ExitCode, errorOutput);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ Failed while attempting to repair moved venv");
                return false;
            }
        }

        private static string? ReadHomePythonFromPyvenvConfig(string pyvenvConfigPath)
        {
            try
            {
                var homeLine = File.ReadLines(pyvenvConfigPath)
                    .FirstOrDefault(line => line.StartsWith("home", StringComparison.OrdinalIgnoreCase));

                if (string.IsNullOrWhiteSpace(homeLine))
                {
                    return null;
                }

                var separatorIndex = homeLine.IndexOf('=');
                if (separatorIndex < 0 || separatorIndex == homeLine.Length - 1)
                {
                    return null;
                }

                var homePath = homeLine[(separatorIndex + 1)..].Trim();
                if (string.IsNullOrWhiteSpace(homePath))
                {
                    return null;
                }

                return Path.Combine(homePath, "python.exe");
            }
            catch
            {
                return null;
            }
        }

        // ============================================================
        // Fallback Response
        // ============================================================
        private PredictionResponse GenerateFallbackResponse(DiseaseType diseaseType, bool isError = false)
        {
            var diseaseName = diseaseType switch
            {
                DiseaseType.HeartDisease => "Bệnh Tim",
                DiseaseType.Diabetes => "Bệnh Tiểu Đường",
                DiseaseType.Hypertension => "Huyết Áp Cao",
                DiseaseType.Stroke => "Đột Quỵ",
                DiseaseType.KidneyStone => "Sỏi Thận",
                DiseaseType.Pneumonia => "Viêm Phổi",
                _ => "Bệnh"
            };

            var diagnostic = GetLastFlaskStartupFailure();
            var technicalNote = string.IsNullOrWhiteSpace(diagnostic)
                ? string.Empty
                : $" Chi tiết kỹ thuật: {diagnostic}";

            return new PredictionResponse
            {
                DiseaseType = diseaseType.ToString(),
                Result = "AI tạm thời không khả dụng",
                RiskLevel = 0,
                Recommendation = string.Empty,
                Details = isError
                    ? $"Không thể kết nối AI backend cho {diseaseName}. Vui lòng chạy AIModel/flask_api.py hoặc kiểm tra service AI rồi thử lại.{technicalNote}"
                    : $"AI đang tắt, không thể đánh giá nguy cơ {diseaseName} lúc này."
            };
        }
    }
}
