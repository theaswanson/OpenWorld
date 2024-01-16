import { useState } from 'react';
import './App.css'
import { HubConnectionBuilder, HubConnectionState } from "@microsoft/signalr";

const buildConnection = () => {
  const connection = new HubConnectionBuilder()
    .withUrl("https://localhost:7192/hubs/chat")
    .build();
  
  connection.on("ReceiveMessage", (user, message) => {
    console.log(`${user}: ${message}`);
  });

  return connection;
};

function App() {
  const [username, setUsername] = useState<string>('');
  const [password, setPassword] = useState<string>('');
  
  const [connection, setConnection] = useState(buildConnection());

  const connect = () => {
    if (connection.state === HubConnectionState.Connected) {
      return;
    }
    
    connection.start()
      .then(() => connection.invoke("SendMessage", "TestUser", "Hello world!"));
  };

  return (
    <div style={{ display: 'flex', flexDirection: 'column' }}>
      <input type="text" placeholder='Username' value={username} onChange={(e) => setUsername(e.target.value)} />
      <input type="text" placeholder='Password' value={password} onChange={(e) => setPassword(e.target.value)} />
      <button onClick={() => connect()}>Connect</button>
    </div>
  );
}

export default App
