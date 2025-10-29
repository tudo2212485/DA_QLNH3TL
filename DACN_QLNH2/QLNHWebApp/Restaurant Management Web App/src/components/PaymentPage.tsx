import React, { useState, useEffect } from 'react';
import { Button } from './ui/button';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { RadioGroup, RadioGroupItem } from './ui/radio-group';
import { Label } from './ui/label';
import { Badge } from './ui/badge';
import { CheckCircle, CreditCard, Smartphone, Building2, Calendar, Clock, Users, Phone } from 'lucide-react';
import { Order } from '../types/index';
import { toast } from 'sonner';

interface PaymentPageProps {
  onPageChange: (page: string) => void;
}

export const PaymentPage: React.FC<PaymentPageProps> = ({ onPageChange }) => {
  const [order, setOrder] = useState<Order | null>(null);
  const [paymentMethod, setPaymentMethod] = useState<'restaurant' | 'ewallet' | 'bank_transfer'>('restaurant');
  const [isProcessing, setIsProcessing] = useState(false);

  useEffect(() => {
    // Load current order from localStorage
    const currentOrder = localStorage.getItem('current_order');
    if (currentOrder) {
      setOrder(JSON.parse(currentOrder));
    } else {
      // No order found, redirect to home
      toast.error('Không tìm thấy đơn hàng. Vui lòng đặt bàn lại.');
      onPageChange('home');
    }
  }, [onPageChange]);

  const handlePayment = async () => {
    if (!order) return;

    setIsProcessing(true);

    try {
      // Simulate payment processing
      await new Promise(resolve => setTimeout(resolve, 2000));

      // Update order with payment method and status
      const updatedOrder = {
        ...order,
        paymentMethod,
        status: 'confirmed' as const
      };

      // Update in localStorage
      const existingOrders = JSON.parse(localStorage.getItem('restaurant_orders') || '[]');
      const orderIndex = existingOrders.findIndex((o: Order) => o.id === order.id);
      if (orderIndex !== -1) {
        existingOrders[orderIndex] = updatedOrder;
        localStorage.setItem('restaurant_orders', JSON.stringify(existingOrders));
      }

      // Clear current order
      localStorage.removeItem('current_order');

      toast.success('Thanh toán thành công! Cảm ơn bạn đã đặt bàn.');
      
      // Redirect to success page or home
      setTimeout(() => {
        onPageChange('home');
      }, 1500);

    } catch (error) {
      toast.error('Có lỗi xảy ra trong quá trình thanh toán. Vui lòng thử lại.');
    } finally {
      setIsProcessing(false);
    }
  };

  if (!order) {
    return (
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="text-center">
          <p>Đang tải thông tin đơn hàng...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900 mb-4">Thanh toán</h1>
        <p className="text-gray-600">
          Xác nhận thông tin và chọn phương thức thanh toán
        </p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
        {/* Payment Methods */}
        <Card>
          <CardHeader>
            <CardTitle>Phương thức thanh toán</CardTitle>
          </CardHeader>
          <CardContent>
            <RadioGroup value={paymentMethod} onValueChange={(value: string) => setPaymentMethod(value as 'restaurant' | 'ewallet' | 'bank_transfer')}>
              <div className="space-y-4">
                {/* Restaurant Payment */}
                <div className="flex items-center space-x-3 border rounded-lg p-4 hover:bg-gray-50">
                  <RadioGroupItem value="restaurant" id="restaurant" />
                  <div className="flex items-center space-x-3 flex-1">
                    <div className="bg-primary/10 rounded-full p-2">
                      <Building2 className="h-5 w-5 text-primary" />
                    </div>
                    <div>
                      <Label htmlFor="restaurant" className="font-medium cursor-pointer">
                        Thanh toán tại nhà hàng
                      </Label>
                      <p className="text-sm text-gray-600">
                        Thanh toán bằng tiền mặt hoặc thẻ khi đến nhà hàng
                      </p>
                    </div>
                  </div>
                </div>

                {/* E-wallet */}
                <div className="flex items-center space-x-3 border rounded-lg p-4 hover:bg-gray-50">
                  <RadioGroupItem value="ewallet" id="ewallet" />
                  <div className="flex items-center space-x-3 flex-1">
                    <div className="bg-green-100 rounded-full p-2">
                      <Smartphone className="h-5 w-5 text-green-600" />
                    </div>
                    <div>
                      <Label htmlFor="ewallet" className="font-medium cursor-pointer">
                        Ví điện tử
                      </Label>
                      <p className="text-sm text-gray-600">
                        MoMo, ZaloPay, VNPay (Mô phỏng)
                      </p>
                    </div>
                  </div>
                </div>

                {/* Bank Transfer */}
                <div className="flex items-center space-x-3 border rounded-lg p-4 hover:bg-gray-50">
                  <RadioGroupItem value="bank_transfer" id="bank_transfer" />
                  <div className="flex items-center space-x-3 flex-1">
                    <div className="bg-blue-100 rounded-full p-2">
                      <CreditCard className="h-5 w-5 text-blue-600" />
                    </div>
                    <div>
                      <Label htmlFor="bank_transfer" className="font-medium cursor-pointer">
                        Chuyển khoản ngân hàng
                      </Label>
                      <p className="text-sm text-gray-600">
                        Chuyển khoản qua Internet Banking
                      </p>
                    </div>
                  </div>
                </div>
              </div>
            </RadioGroup>

            {/* Payment Info for selected method */}
            {paymentMethod === 'bank_transfer' && (
              <div className="mt-6 p-4 bg-blue-50 rounded-lg">
                <h4 className="font-medium mb-2">Thông tin chuyển khoản:</h4>
                <div className="space-y-1 text-sm">
                  <p><strong>Ngân hàng:</strong> Vietcombank</p>
                  <p><strong>Số tài khoản:</strong> 0123456789</p>
                  <p><strong>Chủ tài khoản:</strong> Nhà Hàng 3TL</p>
                  <p><strong>Nội dung:</strong> {order.id}</p>
                </div>
              </div>
            )}

            {paymentMethod === 'ewallet' && (
              <div className="mt-6 p-4 bg-green-50 rounded-lg">
                <h4 className="font-medium mb-2">Hướng dẫn thanh toán:</h4>
                <div className="space-y-1 text-sm">
                  <p>1. Mở ứng dụng ví điện tử</p>
                  <p>2. Quét mã QR hoặc nhập số điện thoại: 0987654321</p>
                  <p>3. Nhập số tiền: {order.totalPrice.toLocaleString('vi-VN')}đ</p>
                  <p>4. Nội dung: Đặt bàn {order.id}</p>
                </div>
              </div>
            )}

            <Button 
              onClick={handlePayment} 
              className="w-full mt-6" 
              size="lg"
              disabled={isProcessing}
            >
              {isProcessing ? (
                <>
                  <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                  Đang xử lý...
                </>
              ) : (
                <>
                  <CheckCircle className="h-5 w-5 mr-2" />
                  Xác nhận thanh toán
                </>
              )}
            </Button>
          </CardContent>
        </Card>

        {/* Order Summary */}
        <div className="space-y-6">
          {/* Booking Details */}
          <Card>
            <CardHeader>
              <CardTitle>Thông tin đặt bàn</CardTitle>
            </CardHeader>
            <CardContent className="space-y-3">
              <div className="flex items-center justify-between">
                <span className="flex items-center text-gray-600">
                  <Phone className="h-4 w-4 mr-2" />
                  Khách hàng
                </span>
                <span className="font-medium">{order.customerName}</span>
              </div>
              
              <div className="flex items-center justify-between">
                <span className="flex items-center text-gray-600">
                  <Phone className="h-4 w-4 mr-2" />
                  Số điện thoại
                </span>
                <span className="font-medium">{order.phone}</span>
              </div>
              
              <div className="flex items-center justify-between">
                <span className="flex items-center text-gray-600">
                  <Calendar className="h-4 w-4 mr-2" />
                  Ngày
                </span>
                <span className="font-medium">{order.date}</span>
              </div>
              
              <div className="flex items-center justify-between">
                <span className="flex items-center text-gray-600">
                  <Clock className="h-4 w-4 mr-2" />
                  Giờ
                </span>
                <span className="font-medium">{order.time}</span>
              </div>
              
              <div className="flex items-center justify-between">
                <span className="flex items-center text-gray-600">
                  <Users className="h-4 w-4 mr-2" />
                  Số khách
                </span>
                <span className="font-medium">{order.guests} khách</span>
              </div>
              
              {order.note && (
                <div className="border-t pt-3">
                  <p className="text-sm text-gray-600">
                    <strong>Ghi chú:</strong> {order.note}
                  </p>
                </div>
              )}
            </CardContent>
          </Card>

          {/* Order Items */}
          <Card>
            <CardHeader>
              <CardTitle>Chi tiết món ăn</CardTitle>
            </CardHeader>
            <CardContent className="space-y-3">
              {(order.orderItems ?? []).map((item) => (
                <div key={item.menuItem.id} className="flex justify-between items-center">
                  <div>
                    <div className="font-medium">{item.menuItem.name}</div>
                    <div className="text-sm text-gray-600">
                      {item.menuItem.price.toLocaleString('vi-VN')}đ x {item.quantity}
                    </div>
                  </div>
                  <div className="font-medium">
                    {(item.menuItem.price * item.quantity).toLocaleString('vi-VN')}đ
                  </div>
                </div>
              ))}
              
              <div className="border-t pt-3">
                <div className="flex justify-between font-bold text-lg">
                  <span>Tổng cộng:</span>
                  <span className="text-primary">
                    {order.totalPrice.toLocaleString('vi-VN')}đ
                  </span>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Order Status */}
          <Card>
            <CardContent className="pt-6">
              <div className="text-center">
                <Badge className="mb-2">
                  Mã đơn hàng: {order.id}
                </Badge>
                <p className="text-sm text-gray-600">
                  Vui lòng lưu lại mã đơn hàng để tra cứu
                </p>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
};