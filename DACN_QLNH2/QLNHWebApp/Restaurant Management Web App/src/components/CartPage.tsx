import React from 'react';
import { Button } from './ui/button';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Badge } from './ui/badge';
import { Minus, Plus, Trash2, ShoppingBag } from 'lucide-react';
import { ImageWithFallback } from './figma/ImageWithFallback';
import { useCart } from '../hooks/useCart';
import { MENU_CATEGORIES } from '../types/index';

interface CartPageProps {
  onPageChange: (page: string) => void;
}

export const CartPage: React.FC<CartPageProps> = ({ onPageChange }) => {
  const { cartItems, updateQuantity, removeFromCart, getTotalPrice, clearCart } = useCart();

  if (cartItems.length === 0) {
    return (
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="text-center py-16">
          <ShoppingBag className="h-24 w-24 text-gray-300 mx-auto mb-4" />
          <h2 className="text-2xl font-bold text-gray-900 mb-4">
            Giỏ hàng của bạn đang trống
          </h2>
          <p className="text-gray-600 mb-8">
            Hãy thêm một số món ăn ngon vào giỏ hàng
          </p>
          <Button size="lg" onClick={() => onPageChange('menu')}>
            Xem thực đơn
          </Button>
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900 mb-4">Giỏ hàng</h1>
        <p className="text-gray-600">
          Xem lại các món ăn bạn đã chọn
        </p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        {/* Cart Items */}
        <div className="lg:col-span-2 space-y-4">
          {cartItems.map((item) => (
            <Card key={item.menuItem.id}>
              <CardContent className="p-4">
                <div className="flex items-center space-x-4">
                  <ImageWithFallback
                    src={item.menuItem.imageUrl}
                    alt={item.menuItem.name}
                    className="w-20 h-20 object-cover rounded-lg"
                  />
                  
                  <div className="flex-1">
                    <h3 className="font-semibold">{item.menuItem.name}</h3>
                    <p className="text-sm text-gray-600 mb-2">
                      {item.menuItem.description}
                    </p>
                    <Badge variant="secondary">
                      {MENU_CATEGORIES?.find(cat => cat.value === item.menuItem.category)?.label || 'Món ăn'}
                    </Badge>
                  </div>
                  
                  <div className="text-right">
                    <div className="font-bold text-lg text-primary mb-2">
                      {(item.menuItem.price * item.quantity).toLocaleString('vi-VN')}đ
                    </div>
                    
                    <div className="flex items-center space-x-2">
                      <Button
                        size="sm"
                        variant="outline"
                        onClick={() => updateQuantity(item.menuItem.id, item.quantity - 1)}
                      >
                        <Minus className="h-3 w-3" />
                      </Button>
                      
                      <span className="w-8 text-center">{item.quantity}</span>
                      
                      <Button
                        size="sm"
                        variant="outline"
                        onClick={() => updateQuantity(item.menuItem.id, item.quantity + 1)}
                      >
                        <Plus className="h-3 w-3" />
                      </Button>
                      
                      <Button
                        size="sm"
                        variant="destructive"
                        onClick={() => removeFromCart(item.menuItem.id)}
                      >
                        <Trash2 className="h-3 w-3" />
                      </Button>
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>
          ))}
          
          <div className="flex justify-between items-center pt-4">
            <Button variant="outline" onClick={clearCart}>
              Xóa tất cả
            </Button>
            <Button variant="outline" onClick={() => onPageChange('menu')}>
              Tiếp tục chọn món
            </Button>
          </div>
        </div>

        {/* Order Summary */}
        <div className="lg:col-span-1">
          <Card className="sticky top-8">
            <CardHeader>
              <CardTitle>Tóm tắt đơn hàng</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="space-y-2">
                {cartItems.map((item) => (
                  <div key={item.menuItem.id} className="flex justify-between text-sm">
                    <span>{item.menuItem.name} x{item.quantity}</span>
                    <span>{(item.menuItem.price * item.quantity).toLocaleString('vi-VN')}đ</span>
                  </div>
                ))}
              </div>
              
              <div className="border-t pt-4">
                <div className="flex justify-between font-bold text-lg">
                  <span>Tổng cộng:</span>
                  <span className="text-primary">
                    {getTotalPrice().toLocaleString('vi-VN')}đ
                  </span>
                </div>
              </div>
              
              <Button 
                className="w-full" 
                size="lg"
                onClick={() => onPageChange('booking')}
              >
                Đặt bàn và thanh toán
              </Button>
              
              <p className="text-xs text-gray-500 text-center">
                * Giá đã bao gồm thuế VAT
              </p>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
};