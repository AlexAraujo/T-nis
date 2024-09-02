import React from 'react';
import './Filtros.css';

const marcas = [
  { value: 'todos', label: 'Todos as marcas' },
  { value: 'casual', label: 'Nike' },
  { value: 'esportivo', label: 'Adidas' },
  { value: 'social', label: 'Vans' }
];
const estilos = [
    { value: 'todos', label: 'Todos os estilos' },
    { value: 'casual', label: 'Casual' },
    { value: 'esportivo', label: 'Esportivo' },
    { value: 'social', label: 'Social' }
]

const Filtros = ({ descricao, setDescricao, cor, setCor, estilo, setEstilo, marca, setMarca, isLoading, lidarComEnvio }) => {
  return (
    <form onSubmit={lidarComEnvio}>
      <div className="inputs-container">
        <input
          className='descricao-input'
          type="text"
          value={descricao}
          onChange={(e) => setDescricao(e.target.value)}
          placeholder="Descrição"
        />
        <div className="color-input">
          <label htmlFor="colorPicker">Cor: </label>
          <input
            id="colorPicker"
            type="color"
            value={cor}
            onChange={(e) => setCor(e.target.value)}
          />
        </div>
        <select value={estilo} onChange={(e) => setEstilo(e.target.value)}>
          {estilos.map((estilo) => (
            <option key={estilo.value} value={estilo.value}>
              {estilo.label}
            </option>
          ))}
        </select>
        <select value={marca} onChange={(e) => setMarca(e.target.value)}>
          {marcas.map((marca) => (
            <option key={marca.value} value={marca.value}>
              {marca.label}
            </option>
          ))}
        </select>
        <button type="submit" disabled={isLoading}>
          {isLoading ? 'Buscando...' : 'Buscar'}
        </button>
      </div>
    </form>
  );
};

export default Filtros;