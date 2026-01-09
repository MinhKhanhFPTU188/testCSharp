import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import api from '../services/api';

const TodoDetail = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [todo, setTodo] = useState(null);
  const [error, setError] = useState('');
  
  // Form state
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [isCompleted, setIsCompleted] = useState(false);

  useEffect(() => {
    const fetchTodo = async () => {
      try {
        const response = await api.get(`/todos/${id}`);
        if (response.data.success) {
          const data = response.data.data;
          setTodo(data);
          setTitle(data.title);
          setDescription(data.description);
          setIsCompleted(data.isCompleted);
        }
      } catch (err) {
        // This handles your specific "sad message" response
        setError(err.response?.data?.message || 'Error fetching todo');
      }
    };
    fetchTodo();
  }, [id]);

  const handleUpdate = async (e) => {
    e.preventDefault();
    try {
      const response = await api.post(`/todos/${id}/update-content`, {
        title,
        description,
        isCompleted
      });
      if (response.data.success) {
        alert('Todo updated successfully');
        navigate('/todos');
      }
    } catch (err) {
      setError(err.response?.data?.message || 'Update failed');
    }
  };

  if (error) {
    return (
      <div className="error-container">
        <h2>Error</h2>
        <p>{error}</p>
        <button onClick={() => navigate('/todos')}>Back to List</button>
      </div>
    );
  }

  if (!todo) return <div>Loading...</div>;

  return (
    <div className="todo-detail-container">
      <h2>Edit Todo</h2>
      <form onSubmit={handleUpdate}>
        <div className="form-group">
          <label>Title</label>
          <input 
            value={title} 
            onChange={(e) => setTitle(e.target.value)} 
            required 
          />
        </div>
        
        <div className="form-group">
          <label>Description</label>
          <textarea 
            value={description} 
            onChange={(e) => setDescription(e.target.value)} 
          />
        </div>

        <div className="form-group checkbox-group">
          <label>
            <input 
              type="checkbox" 
              checked={isCompleted} 
              onChange={(e) => setIsCompleted(e.target.checked)} 
            />
            Mark as Completed
          </label>
        </div>

        <button type="submit">Save Changes</button>
        <button type="button" onClick={() => navigate('/todos')} className="cancel-btn">
          Cancel
        </button>
      </form>
    </div>
  );
};

export default TodoDetail;