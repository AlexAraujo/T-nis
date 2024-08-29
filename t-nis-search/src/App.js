import React, { useState } from 'react';
import axios from 'axios';
import './App.css';

function App() {
  const [inputs, setInputs] = useState(Array(6).fill(''));
  const [images, setImages] = useState([]);
  const [filter, setFilter] = useState('all');

  const handleInputChange = (index, value) => {
    const newInputs = [...inputs];
    newInputs[index] = value;
    setInputs(newInputs);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const response = await axios.post('/api/search', { inputs, filter });
      setImages(response.data.images);
    } catch (error) {
      console.error('Erro ao buscar imagens:', error);
    }
  };

  const handleFilterChange = (e) => {
    setFilter(e.target.value);
  };

  return (
    <div className="App">
      <h1>T-Nis Search</h1>
      <form onSubmit={handleSubmit}>
        <div className="inputs-container">
          {inputs.map((input, index) => (
            <input
              key={index}
              type="text"
              value={input}
              onChange={(e) => handleInputChange(index, e.target.value)}
              placeholder={`Input ${index + 1}`}
            />
          ))}
        </div>
        <div className="filter-container">
          <select value={filter} onChange={handleFilterChange}>
            <option value="all">Todos</option>
            <option value="new">Novos</option>
            <option value="used">Usados</option>
          </select>
        </div>
        <button type="submit">Buscar</button>
      </form>
      <div className="image-grid">
        {images.map((image, index) => (
          <img key={index} src={image.url} alt={image.description} />
        ))}
      </div>
    </div>
  );
}

export default App;
