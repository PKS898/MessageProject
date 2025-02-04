document.addEventListener("DOMContentLoaded", function () {
    if (document.getElementById("messageText")) {
        document.getElementById("messageText").addEventListener("keypress", function (event) {
            if (event.key === "Enter") sendMessage();
        });
    }
});

async function sendMessage() {
    const text = document.getElementById("messageText").value;
    if (!text.trim()) {
        document.getElementById("status").innerText = "Сообщение не может быть пустым.";
        return;
    }

    const message = { sequenceNumber: Math.floor(Math.random() * 1000), text };

    try {
        const response = await fetch("/api/messages", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(message)
        });

        if (response.ok) {
            document.getElementById("status").innerText = "Успешно отправлено!";
            document.getElementById("messageText").value = ""; // Очистка поля
        } else {
            document.getElementById("status").innerText = "Ошибка отправки сообщения.";
        }
    } catch (error) {
        document.getElementById("status").innerText = "Сервер недоступен.";
    }
}

// Подключение к WebSocket (SignalR)
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/messagehub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on("ReceiveMessage", (seq, text, timestamp) => {
    const messagesDiv = document.getElementById("messages");
    if (messagesDiv) {
        const msg = document.createElement("p");
        msg.innerHTML = `<strong>#${seq}</strong> [${new Date(timestamp).toLocaleTimeString()}]: ${text}`;
        messagesDiv.appendChild(msg);
    }
});

connection.start().catch(err => console.error(err));
