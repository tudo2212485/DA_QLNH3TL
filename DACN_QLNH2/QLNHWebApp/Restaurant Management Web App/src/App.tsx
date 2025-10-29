import React, { useState } from 'react';
import { Layout } from './components/Layout';
import { HomePage } from './components/HomePage';
import { MenuPage } from './components/MenuPage';
import { CartPage } from './components/CartPage';
import { BookingPage } from './components/BookingPage';
import { PaymentPage } from './components/PaymentPage';
import { AboutPage } from './components/AboutPage';
import { ContactPage } from './components/ContactPage';
import { Toaster } from './components/ui/sonner';

export default function App() {
  const [currentPage, setCurrentPage] = useState('home');
  const [searchQuery, setSearchQuery] = useState('');

  const renderPage = () => {
    switch (currentPage) {
      case 'home':
        return <HomePage onPageChange={setCurrentPage} />;
      case 'menu':
        return <MenuPage searchQuery={searchQuery} />;
      case 'cart':
        return <CartPage onPageChange={setCurrentPage} />;
      case 'booking':
        return <BookingPage onPageChange={setCurrentPage} />;
      case 'payment':
        return <PaymentPage onPageChange={setCurrentPage} />;
      case 'about':
        return <AboutPage />;
      case 'contact':
        return <ContactPage />;
      default:
        return <HomePage onPageChange={setCurrentPage} />;
    }
  };

  return (
    <div className="min-h-screen bg-background">
      <Layout
        currentPage={currentPage}
        onPageChange={setCurrentPage}
        searchQuery={searchQuery}
        onSearchChange={setSearchQuery}
      >
        {renderPage()}
      </Layout>
      <Toaster position="top-right" />
    </div>
  );
}