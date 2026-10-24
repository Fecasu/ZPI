import React from 'react';
import { createRoot } from 'react-dom/client';
import './style.css';

function App() {
  return (
    <div className="page">
      <header><a href="/" aria-label="WebStore — strona główna"><span className="mark">W</span> WebStore</a><span>Projekt ZPI</span></header>
      <main>
        <p className="eyebrow">W przygotowaniu</p>
        <h1>Tu powstaje<br />Twój nowy sklep.</h1>
        <p className="intro">Pracujemy nad miejscem, w którym wygodnie znajdziesz produkty i złożysz zamówienie.</p>
        <div className="notice"><span className="dot" aria-hidden="true"></span><p>To pierwsza wersja aplikacji. Oferta i zakupy będą dostępne w kolejnych etapach.</p></div>
      </main>
      <footer><span>WebStore</span><span>Sprint 01 · Fundament aplikacji</span></footer>
    </div>
  );
}

createRoot(document.getElementById('root')).render(<React.StrictMode><App /></React.StrictMode>);
