const connectionStatus = document.querySelector("#connection-status");
const statusIndicator = document.querySelector("#status-indicator");

async function checkBackend() {
  try {
    const response = await fetch("/api/health");
    if (!response.ok) {
      throw new Error(`API returned ${response.status}`);
    }

    const health = await response.json();
    connectionStatus.textContent = `${health.application} API is online`;
    statusIndicator.classList.add("connected");
    statusIndicator.setAttribute("aria-label", "Connected");
  } catch (error) {
    connectionStatus.textContent = "Could not reach the C# API";
    statusIndicator.classList.add("error");
    statusIndicator.setAttribute("aria-label", "Disconnected");
    console.error("Backend health check failed:", error);
  }
}

checkBackend();
