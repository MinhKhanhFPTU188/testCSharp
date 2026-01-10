import React, { createContext, useContext, useEffect, useState } from 'react';
import { EventSourcePolyfill } from 'event-source-polyfill';
import api from '../services/api';

const NotificationContext = createContext();

export const useNotification = () => useContext(NotificationContext);

export const NotificationProvider = ({ children }) => {
  const [notification, setNotification] = useState(null); // The text to show in the popup
  const [lastTodoReceived, setLastTodoReceived] = useState(null); // Data for TodoList to use

  useEffect(() => {
    const token = localStorage.getItem('token');
    
    // Only connect if we have a token
    if (!token) return;

    const baseURL = api.defaults.baseURL || 'http://localhost:5257/api';
    
    const eventSource = new EventSourcePolyfill(`${baseURL}/notifications/subscribe`, {
      headers: { 'Authorization': `Bearer ${token}` },
      heartbeatTimeout: 86400000,
    });

    eventSource.onmessage = (event) => {
      try {
        const parsedData = JSON.parse(event.data);

        // Handle TODO_CREATED
        if (parsedData.type === 'TODO_CREATED') {
          const newTodo = parsedData.payload;
          
          // 1. Trigger the Visual Popup
          setNotification(`New task created: "${newTodo.title}"`);
          
          // 2. Store data so TodoList can update itself
          setLastTodoReceived(newTodo);

          // Auto-hide popup after 3 seconds
          setTimeout(() => {
            setNotification(null);
          }, 3000);
        }
      } catch (err) {
        console.error("Error parsing SSE message", err);
      }
    };

    return () => {
      eventSource.close();
    };
  }, []); // Connect once on mount (or you can add token as dependency if auth changes dynamically)

  return (
    <NotificationContext.Provider value={{ lastTodoReceived }}>
      {/* This Render is the Global Popup */}
      {notification && (
        <div className="notification-toast">
          {notification}
        </div>
      )}
      {children}
    </NotificationContext.Provider>
  );
};