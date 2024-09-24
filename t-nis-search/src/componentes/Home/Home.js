import React from 'react';
import './Home.css';

const Home = () => {
  return (
    <div className='home-page'>
      <div className='hero-section'>
        <h1>Seja-vindo ao T-Nis Search</h1>
        <p className='p-home'>Use a navegação acima para explorar o site.</p>
        <p className='p-home'>Este é um site para buscar imagens de alta qualidade.</p>
        <p className='p-home'>É utilizado IA para te ajudar a fazer a busca das imagens que vc deseja.</p>
        <p>Encontre as melhores fotos para seus projetos e inspiração.</p>
      </div>
    </div>
  );
};

export default Home;