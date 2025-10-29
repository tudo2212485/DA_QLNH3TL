import React from 'react';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Badge } from './ui/badge';
import { Search, ShoppingCart, Phone, Mail, MapPin } from 'lucide-react';
import { useCart } from '../hooks/useCart';

interface LayoutProps {
  children: React.ReactNode;
  currentPage: string;
  onPageChange: (page: string) => void;
  searchQuery: string;
  onSearchChange: (query: string) => void;
}

export const Layout: React.FC<LayoutProps> = ({ 
  children, 
  currentPage, 
  onPageChange, 
  searchQuery, 
  onSearchChange 
}) => {
  const { getTotalItems } = useCart();

  const menuItems = [
    { id: 'home', label: 'Trang chủ' },
    { id: 'menu', label: 'Thực đơn' },
    { id: 'booking', label: 'Đặt bàn' },
    { id: 'about', label: 'Giới thiệu' },
    { id: 'contact', label: 'Liên hệ' },
  ];

  return (
    <div className="min-h-screen bg-background">
      {/* Header */}
      <header className="bg-white shadow-sm border-b sticky top-0 z-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center h-16">
            {/* Logo */}
            <div className="flex-shrink-0">
              <h1 className="text-2xl font-bold text-primary">Nhà Hàng 3TL</h1>
            </div>

            {/* Navigation */}
            <nav className="hidden md:flex space-x-8">
              {menuItems.map((item) => (
                <button
                  key={item.id}
                  onClick={() => onPageChange(item.id)}
                  className={`px-3 py-2 rounded-md transition-colors ${
                    currentPage === item.id
                      ? 'text-primary bg-primary/10'
                      : 'text-gray-600 hover:text-primary'
                  }`}
                >
                  {item.label}
                </button>
              ))}
            </nav>

            {/* Cart and Search */}
            <div className="flex items-center space-x-4">
              <div className="relative">
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 h-4 w-4" />
                <Input
                  type="text"
                  placeholder="Tìm kiếm món ăn..."
                  value={searchQuery}
                  onChange={(e) => onSearchChange(e.target.value)}
                  className="pl-10 w-64"
                />
              </div>
              
              <Button
                variant="outline"
                size="sm"
                onClick={() => onPageChange('cart')}
                className="relative"
              >
                <ShoppingCart className="h-4 w-4" />
                {getTotalItems() > 0 && (
                  <Badge className="absolute -top-2 -right-2 h-5 w-5 rounded-full p-0 flex items-center justify-center text-xs">
                    {getTotalItems()}
                  </Badge>
                )}
              </Button>
            </div>
          </div>
        </div>

        {/* Mobile Navigation */}
        <div className="md:hidden border-t">
          <div className="px-4 py-2 space-y-1">
            {menuItems.map((item) => (
              <button
                key={item.id}
                onClick={() => onPageChange(item.id)}
                className={`block w-full text-left px-3 py-2 rounded-md transition-colors ${
                  currentPage === item.id
                    ? 'text-primary bg-primary/10'
                    : 'text-gray-600 hover:text-primary'
                }`}
              >
                {item.label}
              </button>
            ))}
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="flex-1">
        {children}
      </main>

      {/* Footer */}
      <footer className="bg-gray-900 text-white">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            {/* Restaurant Info */}
            <div>
              <h3 className="text-xl font-bold mb-4">Nhà Hàng 3TL</h3>
              <p className="text-gray-300 mb-4">
                Nhà hàng chuyên phục vụ các món ăn truyền thống Việt Nam với hương vị đặc trưng và không gian ấm cúng.
              </p>
            </div>

            {/* Contact Info */}
            <div>
              <h3 className="text-xl font-bold mb-4">Thông tin liên hệ</h3>
              <div className="space-y-2 text-gray-300">
                <div className="flex items-center">
                  <MapPin className="h-4 w-4 mr-2" />
                  <span>123 Đường Nguyễn Huệ, Quận 1, TP.HCM</span>
                </div>
                <div className="flex items-center">
                  <Phone className="h-4 w-4 mr-2" />
                  <span>(028) 3822 3456</span>
                </div>
                <div className="flex items-center">
                  <Mail className="h-4 w-4 mr-2" />
                  <span>info@nhahang-saigon.com</span>
                </div>
              </div>
            </div>

            {/* Operating Hours */}
            <div>
              <h3 className="text-xl font-bold mb-4">Giờ mở cửa</h3>
              <div className="space-y-2 text-gray-300">
                <div>Thứ 2 - Thứ 6: 10:00 - 22:00</div>
                <div>Thứ 7 - Chủ nhật: 9:00 - 23:00</div>
              </div>
            </div>
          </div>
          
          <div className="border-t border-gray-700 mt-8 pt-8 text-center text-gray-400">
            <p>&copy; 2024 Nhà Hàng 3TL. Tất cả các quyền được bảo lưu.</p>
          </div>
        </div>
      </footer>
    </div>
  );
};