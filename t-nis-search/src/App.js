import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './App.css';

const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'https://localhost:7029';

function App() {
  const [descricao, setDescricao] = useState('');
  const [cor, setCor] = useState('');
  const [estilo, setEstilo] = useState('todos');
  const [images, setImages] = useState([]);

  useEffect(() => {
    buscarImagens();
  }, []);

  const buscarImagens = async () => {
    try {
      const response = await axios.get(`${API_BASE_URL}/api/images`);
      setImages(Array.isArray(response.data.images) ? response.data.images : []);
    } catch (error) {
      console.error('Erro ao buscar imagens:', error);
      setImages([]);
    }
  };

  const lidarComEnvio = async (e) => {
    e.preventDefault();
    try {
      const response = await axios.post(`${API_BASE_URL}/api/Search`, { descricao, cor, estilo }, {
        headers: {
          'Content-Type': 'application/json; charset=utf-8'
        }
      });
      setImages(Array.isArray(response.data.images) ? response.data.images : []);
    } catch (error) {
      console.error('Erro ao buscar imagens:', error);
      if (error.response) {
        console.error('Resposta do servidor:', error.response.data);
        console.error('Status do erro:', error.response.status);
      }
      setImages([]);
    }
  };

  return (
    <div className="App">
      <h1>T-Nis Search</h1>
      <form onSubmit={lidarComEnvio}>
        <div className="inputs-container">
          <input
            type="text"
            value={descricao}
            onChange={(e) => setDescricao(e.target.value)}
            placeholder="Descrição"
          />
          <input
            type="text"
            value={cor}
            onChange={(e) => setCor(e.target.value)}
            placeholder="Cor"
          />
          <select value={estilo} onChange={(e) => setEstilo(e.target.value)}>
            <option value="todos">Todos os estilos</option>
            <option value="casual">Casual</option>
            <option value="esportivo">Esportivo</option>
            <option value="social">Social</option>
          </select>
        </div>
        <button type="submit">Buscar</button>
      </form>
      <div className="image-grid">
        {Array.isArray(images) && images.length > 0 ? (
          images.map((image, index) => (
            <div key={index} className="image-item">
              <img 
                src={image.url} 
                alt={image.description} 
                onError={(e) => {
                  console.error('Erro ao carregar imagem:', image.url);
                  e.target.src = 'https://via.placeholder.com/200x200?text=Imagem+não+encontrada';
                }}
              />
              <p className="image-description">{image.description}</p>
            </div>
          ))
        ) : (
          <p>Nenhuma imagem encontrada.</p>
        )}
      </div>
    </div>
  );
}

export default App;
