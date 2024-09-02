import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import Home from './componentes/Home/Home';
import TSearch from './componentes/TSearch/TSearch';
import Sobre from './componentes/Sobre/Sobre';
import Logout from './componentes/Logout/Logout';
import './App.css';

function App() {
  return (
    <Router>
      <div className="App">
        <nav>
          <ul>
            <li><Link to="/">Home</Link></li>
            <li><Link to="/tsearch">T-Search</Link></li>
            <li><Link to="/sobre">Sobre</Link></li>
            <li><Link to="/logout">Logout</Link></li>
          </ul>
        </nav>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/tsearch" element={<TSearch />} />
          <Route path="/sobre" element={<Sobre />} />
          <Route path="/logout" element={<Logout />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;