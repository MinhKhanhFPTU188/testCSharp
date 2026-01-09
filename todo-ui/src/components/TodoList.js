import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import api from '../services/api';

const TodoList = () => {
  const [todos, setTodos] = useState([]);
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    fetchTodos();
  }, []);

  const fetchTodos = async () => {
    try {
      const response = await api.get('/todos');
      if (response.data.success) {
        setTodos(response.data.data);
      }
    } catch (err) {
      if (err.response?.status === 401) navigate('/login');
    }
  };

  const handleCreate = async (e) => {
    e.preventDefault();
    try {
      await api.post('/todos/create', { title, description });
      setTitle('');
      setDescription('');
      fetchTodos(); // Refresh list
    } catch (err) {
      alert('Failed to create todo');
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure?')) return;
    try {
      await api.delete(`/todos/${id}`);
      setTodos(todos.filter(t => t.id !== id));
    } catch (err) {
      alert(err.response?.data?.message || 'Delete failed');
    }
  };

  const handleLogout = () => {
    localStorage.removeItem('token');
    navigate('/login');
  };

  return (
    <div className="todo-container">
      <header className="todo-header">
        <h1>My Todos</h1>
        <button onClick={handleLogout} className="logout-btn">Logout</button>
      </header>

      <div className="create-todo-form">
        <h3>Add New Task</h3>
        <form onSubmit={handleCreate}>
          <input
            placeholder="Title"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            required
          />
          <input
            placeholder="Description"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
          />
          <button type="submit">Add Todo</button>
        </form>
      </div>

      <ul className="todo-list">
        {todos.map(todo => (
          <li key={todo.id} className="todo-item">
            <div className="todo-content">
              <strong>{todo.title}</strong>
              <p>{todo.description}</p>
              <small>Status: {todo.isCompleted ? 'Done' : 'Pending'}</small>
            </div>
            <div className="todo-actions">
              <Link to={`/todos/${todo.id}`}>Edit / Details</Link>
              <button onClick={() => handleDelete(todo.id)} className="delete-btn">Delete</button>
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default TodoList;