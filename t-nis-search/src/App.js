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
        <header>
          <nav>
            <ul className="nav-list">
              <li className="nav-item"><Link to="/">Home</Link></li>
              <li className="nav-item"><Link to="/tsearch">T-Search</Link></li>
              <li className="nav-item"><Link to="/sobre">Sobre</Link></li>
              <li className="nav-item"><Link to="/logout">Logout</Link></li>
            </ul>
          </nav>
        </header>
        <main>
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/tsearch" element={<TSearch />} />
            <Route path="/sobre" element={<Sobre />} />
            <Route path="/logout" element={<Logout />} />
          </Routes>
        </main>
      </div>
    </Router>
  );
}

export default App;