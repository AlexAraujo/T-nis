import React, { useState } from 'react';
import axios from 'axios';
import { showLoadingAlert, showErrorAlert, showSuccessAlert } from '../Alerts/Alerts';
import Filtros from '../Filtros/Filtros';
import './TSearch.css';

const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'https://localhost:7029';

const TSearch = () => {
  const [descricao, setDescricao] = useState('');
  const [cor, setCor] = useState('#000000');
  const [estilo, setEstilo] = useState('todos');
  const [marca, setMarca] = useState('todas');
  const [images, setImages] = useState([]);
  const [isLoading, setIsLoading] = useState(false);

  const lidarComEnvio = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    showLoadingAlert();

    try {
      const response = await axios.post(`${API_BASE_URL}/api/Unsplash/search`, 
        { descricao, cor, estilo }, 
        {
          headers: {
            'Content-Type': 'application/json'
          }
        }
      );

      if (response.data && response.data.images) {
        setImages(response.data.images);
        showSuccessAlert('Busca concluída com sucesso.');
      } else {
        setImages([]);
        showErrorAlert('Nenhuma imagem encontrada.');
      }
    } catch (error) {
      showErrorAlert('Ocorreu um erro ao buscar as imagens.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="TSearch">
      <Filtros
        descricao={descricao}
        setDescricao={setDescricao}
        cor={cor}
        setCor={setCor}
        estilo={estilo}
        setEstilo={setEstilo}
        marca={marca}
        setMarca={setMarca}
        isLoading={isLoading}
        lidarComEnvio={lidarComEnvio}
      />
      <div className="image-grid">
        {Array.isArray(images) && images.length > 0 ? (
          images.map((image, index) => (
            <div key={index} className="image-item">
              <img 
                src={image.url} 
                alt={image.descricao} 
                onError={(e) => {
                  console.error('Erro ao carregar imagem:', image.url);
                  e.target.src = 'https://via.placeholder.com/200x200?text=Imagem+não+encontrada';
                }}
              />
              <p className="image-description">{image.descricao}</p>
              <p className="image-color">Cor: {image.cor}</p>
              <p className="image-style">Estilo: {image.estilo}</p>
            </div>
          ))
        ) : (
          <p>Nenhuma imagem encontrada.</p>
        )}
      </div>
    </div>
  );
};

export default TSearch;