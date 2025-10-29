import React, { useState, useEffect } from 'react';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Textarea } from './ui/textarea';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Calendar, Clock, Users, Phone, User, MapPin, Table as TableIcon } from 'lucide-react';
import { useCart } from '../hooks/useCart';
import { Order } from '../types/index';
import { toast } from 'sonner';
import { apiService } from '../services/api';

interface BookingPageProps {
  onPageChange: (page: string) => void;
}

interface Table {
  id: number;
  name: string;
  floor: string;
  capacity: number;
  description: string;
  imageUrl: string;
  isAvailable: boolean;
}

export const BookingPage: React.FC<BookingPageProps> = ({ onPageChange }) => {
  const { cartItems, getTotalPrice, clearCart } = useCart();
  const [formData, setFormData] = useState({
    customerName: '',
    phone: '',
    email: '',
    date: '',
    time: '',
    guests: '',
    note: ''
  });
  const [loading, setLoading] = useState(false);
  const [showTableSelection, setShowTableSelection] = useState(false);
  const [selectedTable, setSelectedTable] = useState<Table | null>(null);
  const [availableTables, setAvailableTables] = useState<Table[]>([]);
  const [loadingTables, setLoadingTables] = useState(false);
  const [selectedFloor, setSelectedFloor] = useState<string | null>(null);
  const [showFloorSelection, setShowFloorSelection] = useState(false);

  const handleInputChange = (field: string, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }));
  };

  // Load available tables when form data changes
  useEffect(() => {
    if (formData.date && formData.time && formData.guests) {
      loadAvailableTables();
    }
  }, [formData.date, formData.time, formData.guests]);

  const loadAvailableTables = async () => {
    if (!formData.date || !formData.time || !formData.guests) return;

    setLoadingTables(true);
    try {
      // Get available floors based on guest count
      const floors = getAvailableFloors(parseInt(formData.guests));

      // Load tables for each floor
      const allTables: Table[] = [];
      for (const floor of floors) {
        try {
          const response = await fetch(`/api/tableapi/GetTablesByFloor?floor=${encodeURIComponent(floor)}&guests=${formData.guests}`);
          if (response.ok) {
            const tables = await response.json();
            const tablesWithAvailability = await Promise.all(
              tables.map(async (table: any) => {
                const isAvailable = await checkTableAvailability(table.id, formData.date, formData.time);
                return {
                  ...table,
                  isAvailable
                };
              })
            );
            allTables.push(...tablesWithAvailability);
          }
        } catch (error) {
          console.error(`Error loading tables for floor ${floor}:`, error);
        }
      }

      setAvailableTables(allTables);
    } catch (error) {
      console.error('Error loading tables:', error);
      toast.error('Không thể tải danh sách bàn');
    } finally {
      setLoadingTables(false);
    }
  };

  const getAvailableFloors = (guests: number): string[] => {
    if (guests <= 4) {
      return ['Tầng 1', 'Tầng 2', 'Sân thượng'];
    } else if (guests <= 8) {
      return ['Tầng 1', 'Tầng 2'];
    } else if (guests <= 15) {
      return ['Tầng 1', 'Tầng 2'];
    } else if (guests <= 20) {
      return ['Tầng 1'];
    }
    return ['Tầng 1'];
  };

  const checkTableAvailability = async (tableId: number, date: string, time: string): Promise<boolean> => {
    try {
      const response = await fetch(`/api/tableapi/CheckTableAvailability?tableId=${tableId}&bookingDate=${date}&bookingTime=${time}`);
      if (response.ok) {
        const result = await response.json();
        return result.isAvailable;
      }
      return false;
    } catch (error) {
      console.error('Error checking table availability:', error);
      return false;
    }
  };

  const handleSelectTable = () => {
    if (!formData.date || !formData.time || !formData.guests) {
      toast.error('Vui lòng điền đầy đủ thông tin ngày, giờ và số khách trước khi chọn bàn');
      return;
    }
    setShowFloorSelection(true);
  };

  const handleSelectFloor = async (floor: string) => {
    setSelectedFloor(floor);
    setShowFloorSelection(false);
    setShowTableSelection(true);
    setLoadingTables(true);

    try {
      const response = await fetch(`/api/tableapi/GetTablesByFloor?floor=${encodeURIComponent(floor)}&guests=${formData.guests}`);
      if (response.ok) {
        const result = await response.json();
        const tables = result.tables || [];

        // Check availability for each table
        const tablesWithAvailability = await Promise.all(
          tables.map(async (table: any) => {
            const isAvailable = await checkTableAvailability(table.id, formData.date, formData.time);
            return {
              ...table,
              isAvailable
            };
          })
        );

        setAvailableTables(tablesWithAvailability);
      } else {
        toast.error('Không thể tải danh sách bàn');
        setAvailableTables([]);
      }
    } catch (error) {
      console.error('Error loading tables:', error);
      toast.error('Không thể tải danh sách bàn');
      setAvailableTables([]);
    } finally {
      setLoadingTables(false);
    }
  };

  const handleSaveAndContinue = async () => {
    // Validate required fields
    if (!formData.customerName || !formData.phone || !formData.date || !formData.time || !formData.guests) {
      toast.error('Vui lòng điền đầy đủ thông tin bắt buộc');
      return;
    }

    try {
      // Save booking info to localStorage for React app
      localStorage.setItem('bookingInfo', JSON.stringify(formData));

      // Also save to backend session via API call
      await apiService.saveBookingInfo(formData);

      toast.success('Thông tin đặt bàn đã được lưu');
      onPageChange('menu');
    } catch (error) {
      console.error('Error saving booking info:', error);
      // Even if API fails, still save to localStorage and continue
      localStorage.setItem('bookingInfo', JSON.stringify(formData));
      toast.success('Thông tin đặt bàn đã được lưu tạm thời');
      onPageChange('menu');
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    // Validate required fields
    if (!formData.customerName || !formData.phone || !formData.date || !formData.time || !formData.guests) {
      toast.error('Vui lòng điền đầy đủ thông tin bắt buộc');
      return;
    }

    // Check if table is selected
    if (!selectedTable) {
      toast.error('Vui lòng chọn bàn trước khi đặt bàn');
      return;
    }

    setLoading(true);

    try {
      // First, create table booking WITH order items
      const bookingResponse = await fetch('/api/tableapi/BookTable', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          tableId: selectedTable.id,
          customerName: formData.customerName,
          phone: formData.phone,
          email: formData.email,
          bookingDate: formData.date,
          bookingTime: formData.time,
          guests: parseInt(formData.guests),
          note: formData.note,
          orderItems: cartItems.map(item => ({
            menuItemId: item.menuItem.id,
            quantity: item.quantity
          }))
        })
      });

      if (!bookingResponse.ok) {
        const errorData = await bookingResponse.json();
        throw new Error(errorData.message || 'Không thể đặt bàn');
      }

      const bookingResult = await bookingResponse.json();

      if (cartItems.length > 0) {
        // Create order with items and link to table booking
        const order = await apiService.createOrder({
          customerName: formData.customerName,
          phone: formData.phone,
          date: formData.date,
          time: formData.time,
          guests: parseInt(formData.guests),
          note: formData.note,
          paymentMethod: 'restaurant',
          tableId: selectedTable.id,
          items: cartItems.map(item => ({
            menuItemId: item.menuItem.id,
            quantity: item.quantity
          }))
        });

        // Store current order for payment page
        localStorage.setItem('current_order', JSON.stringify(order));
      } else {
        // Store booking info for payment page
        const bookingInfo = {
          ...formData,
          tableId: selectedTable.id,
          tableName: selectedTable.name,
          tableFloor: selectedTable.floor,
          bookingId: bookingResult.bookingId
        };
        localStorage.setItem('current_order', JSON.stringify(bookingInfo));
      }

      // Clear cart
      clearCart();

      toast.success(`Đặt bàn thành công! Bàn ${selectedTable.name} đã được chọn. Đang chuyển đến trang thanh toán...`);

      // Redirect to server-rendered Payment page (relative path, no localhost hardcode)
      window.location.assign('/Payment');
    } catch (error) {
      console.error('Error creating booking:', error);
      toast.error('Có lỗi xảy ra khi đặt bàn. Vui lòng thử lại.');
    } finally {
      setLoading(false);
    }
  };

  // Generate time options
  const timeOptions = [];
  for (let hour = 10; hour <= 22; hour++) {
    for (let minute = 0; minute < 60; minute += 30) {
      const timeString = `${hour.toString().padStart(2, '0')}:${minute.toString().padStart(2, '0')}`;
      timeOptions.push(timeString);
    }
  }

  // Generate guest options
  const guestOptions = Array.from({ length: 20 }, (_, i) => i + 1);

  return (
    <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900 mb-4">Đặt bàn</h1>
        <p className="text-gray-600">
          Vui lòng điền thông tin để hoàn tất đặt bàn
        </p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
        {/* Booking Form */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center">
              <User className="h-5 w-5 mr-2" />
              Thông tin đặt bàn
            </CardTitle>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="space-y-6">
              {/* Customer Name */}
              <div className="space-y-2">
                <Label htmlFor="customerName">
                  Tên khách hàng <span className="text-red-500">*</span>
                </Label>
                <Input
                  id="customerName"
                  type="text"
                  value={formData.customerName}
                  onChange={(e) => handleInputChange('customerName', e.target.value)}
                  placeholder="Nhập tên đầy đủ"
                  required
                />
              </div>

              {/* Phone */}
              <div className="space-y-2">
                <Label htmlFor="phone">
                  Số điện thoại <span className="text-red-500">*</span>
                </Label>
                <div className="relative">
                  <Phone className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 h-4 w-4" />
                  <Input
                    id="phone"
                    type="tel"
                    value={formData.phone}
                    onChange={(e) => handleInputChange('phone', e.target.value)}
                    placeholder="Nhập số điện thoại"
                    className="pl-10"
                    required
                  />
                </div>
              </div>

              {/* Email */}
              <div className="space-y-2">
                <Label htmlFor="email">
                  Email
                </Label>
                <Input
                  id="email"
                  type="email"
                  value={formData.email}
                  onChange={(e) => handleInputChange('email', e.target.value)}
                  placeholder="email@example.com (để nhận thông báo đơn hàng)"
                />
              </div>

              {/* Date */}
              <div className="space-y-2">
                <Label htmlFor="date">
                  Ngày đặt bàn <span className="text-red-500">*</span>
                </Label>
                <div className="relative">
                  <Calendar className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 h-4 w-4" />
                  <Input
                    id="date"
                    type="date"
                    value={formData.date}
                    onChange={(e) => handleInputChange('date', e.target.value)}
                    className="pl-10"
                    min={new Date().toISOString().split('T')[0]}
                    required
                  />
                </div>
              </div>

              {/* Time */}
              <div className="space-y-2">
                <Label htmlFor="time">
                  Giờ đặt bàn <span className="text-red-500">*</span>
                </Label>
                <Select value={formData.time} onValueChange={(value: string) => handleInputChange('time', value)}>
                  <SelectTrigger>
                    <div className="flex items-center">
                      <Clock className="h-4 w-4 mr-2 text-gray-400" />
                      <SelectValue placeholder="Chọn giờ" />
                    </div>
                  </SelectTrigger>
                  <SelectContent>
                    {timeOptions.map((time) => (
                      <SelectItem key={time} value={time}>
                        {time}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              {/* Number of Guests */}
              <div className="space-y-2">
                <Label htmlFor="guests">
                  Số khách <span className="text-red-500">*</span>
                </Label>
                <Select value={formData.guests} onValueChange={(value: string) => handleInputChange('guests', value)}>
                  <SelectTrigger>
                    <div className="flex items-center">
                      <Users className="h-4 w-4 mr-2 text-gray-400" />
                      <SelectValue placeholder="Chọn số khách" />
                    </div>
                  </SelectTrigger>
                  <SelectContent>
                    {guestOptions.map((num) => (
                      <SelectItem key={num} value={num.toString()}>
                        {num} {num === 1 ? 'khách' : 'khách'}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              {/* Note */}
              <div className="space-y-2">
                <Label htmlFor="note">Ghi chú</Label>
                <Textarea
                  id="note"
                  value={formData.note}
                  onChange={(e) => handleInputChange('note', e.target.value)}
                  placeholder="Yêu cầu đặc biệt (tùy chọn)"
                  rows={3}
                />
              </div>

              {/* Table Selection */}
              <div className="space-y-3 p-4 border-2 border-green-200 rounded-lg bg-green-50">
                <div className="flex items-center">
                  <TableIcon className="h-5 w-5 mr-2 text-green-600" />
                  <Label className="text-green-700 font-semibold">Chọn bàn phù hợp</Label>
                </div>

                {selectedTable ? (
                  <div className="p-3 bg-white rounded-md border border-green-300">
                    <div className="flex items-center justify-between">
                      <div>
                        <div className="font-medium text-green-800">{selectedTable.name}</div>
                        <div className="text-sm text-green-600 flex items-center">
                          <MapPin className="h-3 w-3 mr-1" />
                          {selectedTable.floor} • {selectedTable.capacity} chỗ ngồi
                        </div>
                      </div>
                      <Button
                        type="button"
                        variant="outline"
                        size="sm"
                        onClick={() => setSelectedTable(null)}
                        className="text-red-600 border-red-300 hover:bg-red-50"
                      >
                        Bỏ chọn
                      </Button>
                    </div>
                  </div>
                ) : (
                  <Button
                    type="button"
                    variant="outline"
                    className="w-full border-green-300 text-green-700 hover:bg-green-100"
                    onClick={handleSelectTable}
                    disabled={!formData.date || !formData.time || !formData.guests}
                  >
                    <TableIcon className="h-4 w-4 mr-2" />
                    {!formData.date || !formData.time || !formData.guests
                      ? 'Vui lòng điền đầy đủ thông tin trước'
                      : 'Chọn bàn phù hợp'}
                  </Button>
                )}
              </div>

              <div className="space-y-3">
                <Button
                  type="button"
                  variant="outline"
                  className="w-full"
                  size="lg"
                  onClick={handleSaveAndContinue}
                  disabled={loading}
                >
                  Lưu thông tin và tiếp tục chọn món
                </Button>

                <Button type="submit" className="w-full" size="lg" disabled={loading}>
                  {loading ? 'Đang xử lý...' : 'Xác nhận đặt bàn'}
                </Button>
              </div>
            </form>
          </CardContent>
        </Card>

        {/* Order Summary */}
        <Card>
          <CardHeader>
            <CardTitle>Chi tiết đơn hàng</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            {cartItems.length > 0 ? (
              <>
                <div className="space-y-3">
                  {cartItems.map((item) => (
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
                </div>

                <div className="border-t pt-4">
                  <div className="flex justify-between font-bold text-lg">
                    <span>Tổng cộng:</span>
                    <span className="text-primary">
                      {getTotalPrice().toLocaleString('vi-VN')}đ
                    </span>
                  </div>
                </div>
              </>
            ) : (
              <div className="text-center py-8">
                <p className="text-gray-500 mb-4">Chưa có món ăn nào được chọn</p>
                <Button onClick={() => onPageChange('menu')}>
                  Chọn món ăn
                </Button>
              </div>
            )}

            <div className="text-sm text-gray-500 space-y-1">
              <p>• Giá đã bao gồm thuế VAT</p>
              <p>• Nhà hàng mở cửa từ 10:00 - 22:00</p>
              <p>• Liên hệ (028) 3822 3456 để hỗ trợ</p>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Floor Selection Modal */}
      {showFloorSelection && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <Card className="w-full max-w-2xl">
            <CardHeader className="flex flex-row items-center justify-between">
              <CardTitle className="flex items-center">
                <MapPin className="h-5 w-5 mr-2" />
                Chọn Tầng
              </CardTitle>
              <Button
                variant="ghost"
                size="sm"
                onClick={() => setShowFloorSelection(false)}
              >
                ✕
              </Button>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                <div className="bg-blue-50 p-4 rounded-lg border border-blue-200">
                  <div className="flex items-center text-blue-800">
                    <Users className="h-5 w-5 mr-2" />
                    <span className="font-medium">Số khách: {formData.guests} người</span>
                  </div>
                  <p className="text-sm text-blue-600 mt-1">
                    Hệ thống sẽ gợi ý các tầng phù hợp với số lượng khách của bạn
                  </p>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                  {/* Tầng 1 */}
                  <div
                    className="p-6 border-2 border-gray-200 rounded-lg cursor-pointer hover:border-blue-500 hover:bg-blue-50 transition-all"
                    onClick={() => handleSelectFloor('Tầng 1')}
                  >
                    <div className="text-center">
                      <div className="w-16 h-16 bg-blue-100 rounded-full flex items-center justify-center mx-auto mb-3">
                        <span className="text-2xl font-bold text-blue-600">1</span>
                      </div>
                      <h3 className="font-semibold text-lg mb-2">Tầng 1</h3>
                      <p className="text-sm text-gray-600 mb-1">8 bàn</p>
                      <p className="text-xs text-gray-500">Phù hợp 1-20 khách</p>
                    </div>
                  </div>

                  {/* Tầng 2 */}
                  <div
                    className="p-6 border-2 border-gray-200 rounded-lg cursor-pointer hover:border-green-500 hover:bg-green-50 transition-all"
                    onClick={() => handleSelectFloor('Tầng 2')}
                  >
                    <div className="text-center">
                      <div className="w-16 h-16 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-3">
                        <span className="text-2xl font-bold text-green-600">2</span>
                      </div>
                      <h3 className="font-semibold text-lg mb-2">Tầng 2</h3>
                      <p className="text-sm text-gray-600 mb-1">7 bàn</p>
                      <p className="text-xs text-gray-500">Phù hợp 1-15 khách</p>
                    </div>
                  </div>

                  {/* Sân thượng */}
                  <div
                    className="p-6 border-2 border-gray-200 rounded-lg cursor-pointer hover:border-yellow-500 hover:bg-yellow-50 transition-all"
                    onClick={() => handleSelectFloor('Sân thượng')}
                  >
                    <div className="text-center">
                      <div className="w-16 h-16 bg-yellow-100 rounded-full flex items-center justify-center mx-auto mb-3">
                        <span className="text-2xl">🌤️</span>
                      </div>
                      <h3 className="font-semibold text-lg mb-2">Sân thượng</h3>
                      <p className="text-sm text-gray-600 mb-1">4 bàn</p>
                      <p className="text-xs text-gray-500">Phù hợp 1-10 khách</p>
                    </div>
                  </div>
                </div>

                <div className="flex justify-end mt-6">
                  <Button
                    variant="outline"
                    onClick={() => setShowFloorSelection(false)}
                  >
                    Đóng
                  </Button>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>
      )}

      {/* Table Selection Modal */}
      {showTableSelection && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <Card className="w-full max-w-4xl max-h-[90vh] overflow-hidden">
            <CardHeader>
              <div className="flex items-center justify-between">
                <CardTitle className="flex items-center">
                  <TableIcon className="h-5 w-5 mr-2" />
                  Chọn bàn - {selectedFloor}
                </CardTitle>
                <Button
                  variant="ghost"
                  size="sm"
                  onClick={() => setShowTableSelection(false)}
                >
                  ✕
                </Button>
              </div>
              <div className="flex items-center gap-2 mt-2">
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => {
                    setShowTableSelection(false);
                    setShowFloorSelection(true);
                  }}
                >
                  <MapPin className="h-4 w-4 mr-1" />
                  Chọn tầng khác
                </Button>
                <span className="text-sm text-gray-500">
                  {formData.guests} khách • {formData.date} • {formData.time}
                </span>
              </div>
            </CardHeader>
            <CardContent className="overflow-y-auto">
              {loadingTables ? (
                <div className="text-center py-8">
                  <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary mx-auto"></div>
                  <p className="mt-2 text-gray-600">Đang tải danh sách bàn...</p>
                </div>
              ) : availableTables.length === 0 ? (
                <div className="text-center py-8">
                  <TableIcon className="h-12 w-12 text-gray-400 mx-auto mb-4" />
                  <p className="text-gray-600">Không có bàn nào phù hợp</p>
                  <p className="text-sm text-gray-500 mt-1">
                    Vui lòng thử ngày/giờ khác hoặc giảm số lượng khách
                  </p>
                </div>
              ) : (
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                  {availableTables.map((table) => (
                    <div
                      key={table.id}
                      className={`p-4 border-2 rounded-lg cursor-pointer transition-all ${table.isAvailable
                        ? selectedTable?.id === table.id
                          ? 'border-green-500 bg-green-50'
                          : 'border-gray-200 hover:border-green-300 hover:bg-green-50'
                        : 'border-red-200 bg-red-50 cursor-not-allowed opacity-60'
                        }`}
                      onClick={() => {
                        if (table.isAvailable) {
                          setSelectedTable(table);
                          setShowTableSelection(false);
                        }
                      }}
                    >
                      <div className="flex items-center justify-between mb-2">
                        <h3 className="font-semibold">{table.name}</h3>
                        <div className={`px-2 py-1 rounded-full text-xs ${table.isAvailable ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                          }`}>
                          {table.isAvailable ? 'Có sẵn' : 'Đã đặt'}
                        </div>
                      </div>
                      <div className="flex items-center text-sm text-gray-600 mb-2">
                        <MapPin className="h-3 w-3 mr-1" />
                        {table.floor}
                      </div>
                      <div className="flex items-center text-sm text-gray-600">
                        <Users className="h-3 w-3 mr-1" />
                        {table.capacity} chỗ ngồi
                      </div>
                      {table.description && (
                        <p className="text-xs text-gray-500 mt-2">{table.description}</p>
                      )}
                    </div>
                  ))}
                </div>
              )}

              <div className="mt-6 flex justify-end">
                <Button
                  variant="outline"
                  onClick={() => setShowTableSelection(false)}
                >
                  Đóng
                </Button>
              </div>
            </CardContent>
          </Card>
        </div>
      )}
    </div>
  );
};