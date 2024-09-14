// Initialize SignalR connection
const connection = new signalR.HubConnectionBuilder()
  .withUrl("/notificationHub")
  .build();

// Function to load messages from local storage and display them
function loadMessages() {
  const messages = document.getElementById("messages");
  const storedMessages = JSON.parse(localStorage.getItem("messages")) || [];
  storedMessages.forEach((message) => {
    const messageElement = document.createElement("p");
    messageElement.textContent = message;
    messages.appendChild(messageElement);
  });
}

// Function to save a message to local storage
function saveMessage(message) {
  const storedMessages = JSON.parse(localStorage.getItem("messages")) || [];
  storedMessages.push(message);
  localStorage.setItem("messages", JSON.stringify(storedMessages));
}

// Function to clear messages from local storage and the page
function clearMessages() {
  localStorage.removeItem("messages");
  document.getElementById("messages").innerHTML = "";
}

// Handle incoming messages
connection.on("receiveMessage", function (message) {
  const messages = document.getElementById("messages");
  const messageElement = document.createElement("p");
  messageElement.textContent = message;
  messages.appendChild(messageElement);
  messages.scrollTop = messages.scrollHeight; // Scroll to the bottom to show the latest message
  saveMessage(message); // Save message to local storage
});

// Start the connection
connection
  .start()
  .catch((err) => console.error("Error while starting connection: " + err));

// Load messages when the page loads
window.onload = loadMessages;

// Attach event listener to clear messages button
document
  .getElementById("clearMessages")
  .addEventListener("click", clearMessages);
