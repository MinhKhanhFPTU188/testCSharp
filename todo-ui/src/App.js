import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import Login from './components/Login';
import Register from './components/Register';
import TodoList from './components/TodoList';
import TodoDetail from './components/TodoDetail';
import { NotificationProvider } from './context/NotificationContext'; // Import this
import './App.css';

function App() {
  const isAuthenticated = !!localStorage.getItem('token');

  return (
    <BrowserRouter>
      {/* Wrap everything inside the Provider */}
      <NotificationProvider>
        <div className="App">
          <Routes>
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            
            <Route 
              path="/todos" 
              element={isAuthenticated ? <TodoList /> : <Navigate to="/login" />} 
            />
            
            <Route 
              path="/todos/:id" 
              element={isAuthenticated ? <TodoDetail /> : <Navigate to="/login" />} 
            />
            
            <Route path="*" element={<Navigate to="/todos" />} />
          </Routes>
        </div>
      </NotificationProvider>
    </BrowserRouter>
  );
}

export default App;