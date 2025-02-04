async function loadMessages() {
    const historyDiv = document.getElementById("history");
    historyDiv.innerHTML = "Loading...";

    const now = new Date();
    const tenMinutesAgo = new Date(now - 10 * 60000); // 10 минут назад

    const from = tenMinutesAgo.toISOString();
    const to = now.toISOString();

    try {
        const response = await fetch(`/api/messages?from=${from}&to=${to}`);
        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }

        const messages = await response.json();
        historyDiv.innerHTML = "";

        if (messages.length === 0) {
            historyDiv.innerHTML = "<p>No messages found.</p>";
        } else {
            messages.forEach(msg => {
                const p = document.createElement("p");
                p.innerHTML = `<strong>#${msg.sequenceNumber}</strong> [${new Date(msg.timestamp).toLocaleTimeString()}]: ${msg.text}`;
                historyDiv.appendChild(p);
            });
        }
    } catch (error) {
        console.error("Error loading messages:", error);
        historyDiv.innerHTML = "<p>Error loading messages.</p>";
    }
}
