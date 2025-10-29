import React, { useState, useEffect } from 'react';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Textarea } from './ui/textarea';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Calendar, Clock, Users, Phone, User, MapPin, Table as TableIcon, Star, CheckCircle2, XCircle } from 'lucide-react';
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

interface FloorInfo {
    name: string;
    icon: string;
    color: string;
    hoverColor: string;
    bgColor: string;
    tables: number;
    capacity: string;
    recommended: boolean;
    disabled: boolean;
}

export const BookingPage: React.FC<BookingPageProps> = ({ onPageChange }) => {
    const { cartItems, getTotalPrice, clearCart } = useCart();
    const [formData, setFormData] = useState({
        customerName: '',
        phone: '',
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
    const [floorTablesCount, setFloorTablesCount] = useState<{ [key: string]: number }>({
        'Tầng 1': 0,
        'Tầng 2': 0,
        'Sân thượng': 0
    });

    const handleInputChange = (field: string, value: string) => {
        setFormData(prev => ({ ...prev, [field]: value }));
    };

    // Load available tables when form data changes
    useEffect(() => {
        if (formData.date && formData.time && formData.guests) {
            loadAllFloorsTablesCount();
        }
    }, [formData.date, formData.time, formData.guests]);

    const loadAllFloorsTablesCount = async () => {
        if (!formData.guests) return;

        const floors = ['Tầng 1', 'Tầng 2', 'Sân thượng'];
        const counts: { [key: string]: number } = {};

        for (const floor of floors) {
            try {
                const response = await fetch(`/api/tableapi/GetTablesByFloor?floor=${encodeURIComponent(floor)}&guests=${formData.guests}`);
                if (response.ok) {
                    const tables = await response.json();
                    counts[floor] = tables.length || 0;
                } else {
                    counts[floor] = 0;
                }
            } catch (error) {
                console.error(`Error loading tables for ${floor}:`, error);
                counts[floor] = 0;
            }
        }

        setFloorTablesCount(counts);
    };

    const getFloorInfo = (guests: number): FloorInfo[] => {
        const guestNum = guests;

        return [
            {
                name: 'Tầng 1',
                icon: '🏢',
                color: 'blue',
                hoverColor: 'hover:border-blue-500 hover:shadow-blue-200',
                bgColor: 'bg-gradient-to-br from-blue-50 to-blue-100',
                tables: floorTablesCount['Tầng 1'] || 8,
                capacity: '1-20 khách',
                recommended: guestNum >= 10,
                disabled: false
            },
            {
                name: 'Tầng 2',
                icon: '🏬',
                color: 'green',
                hoverColor: 'hover:border-green-500 hover:shadow-green-200',
                bgColor: 'bg-gradient-to-br from-green-50 to-green-100',
                tables: floorTablesCount['Tầng 2'] || 7,
                capacity: '1-15 khách',
                recommended: guestNum >= 5 && guestNum < 10,
                disabled: guestNum > 15
            },
            {
                name: 'Sân thượng',
                icon: '🌤️',
                color: 'amber',
                hoverColor: 'hover:border-amber-500 hover:shadow-amber-200',
                bgColor: 'bg-gradient-to-br from-amber-50 to-amber-100',
                tables: floorTablesCount['Sân thượng'] || 4,
                capacity: '1-10 khách',
                recommended: guestNum <= 4,
                disabled: guestNum > 10
            }
        ];
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

    const handleSelectFloor = async (floor: string, disabled: boolean) => {
        if (disabled) {
            toast.error('Tầng này không phù hợp với số lượng khách của bạn');
            return;
        }

        setSelectedFloor(floor);
        setShowFloorSelection(false);
        setShowTableSelection(true);
        setLoadingTables(true);

        try {
            const response = await fetch(`/api/tableapi/GetTablesByFloor?floor=${encodeURIComponent(floor)}&guests=${formData.guests}`);
            if (response.ok) {
                const tables = await response.json();

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
            toast.error('Có lỗi xảy ra khi tải danh sách bàn');
            setAvailableTables([]);
        } finally {
            setLoadingTables(false);
        }
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!selectedTable) {
            toast.error('Vui lòng chọn bàn');
            return;
        }

        if (!formData.customerName || !formData.phone || !formData.date || !formData.time || !formData.guests) {
            toast.error('Vui lòng điền đầy đủ thông tin');
            return;
        }

        setLoading(true);

        try {
            const orderData: Order = {
                id: Date.now().toString(),
                customerName: formData.customerName,
                phone: formData.phone,
                date: formData.date,
                time: formData.time,
                guests: parseInt(formData.guests),
                note: formData.note,
                tableId: selectedTable.id,
                tableName: selectedTable.name,
                tableFloor: selectedTable.floor,
                orderItems: cartItems,
                totalPrice: getTotalPrice(),
                status: 'pending'
            };

            // Save order to localStorage
            const existingOrders = JSON.parse(localStorage.getItem('restaurant_orders') || '[]');
            existingOrders.push(orderData);
            localStorage.setItem('restaurant_orders', JSON.stringify(existingOrders));
            localStorage.setItem('current_order', JSON.stringify(orderData));

            clearCart();
            toast.success('Đặt bàn thành công! Chuyển đến trang thanh toán...');

            setTimeout(() => {
                onPageChange('payment');
            }, 1000);

        } catch (error) {
            console.error('Booking error:', error);
            toast.error('Có lỗi xảy ra khi đặt bàn. Vui lòng thử lại.');
        } finally {
            setLoading(false);
        }
    };

    const floors = formData.guests ? getFloorInfo(parseInt(formData.guests)) : [];

    return (
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
            <div className="mb-8">
                <h1 className="text-3xl font-bold text-gray-900 mb-4">Đặt bàn</h1>
                <p className="text-gray-600">
                    Vui lòng điền thông tin để hoàn tất đặt bàn
                </p>
            </div>

            <form onSubmit={handleSubmit} className="grid grid-cols-1 lg:grid-cols-3 gap-8">
                {/* Form Column */}
                <div className="lg:col-span-2 space-y-6">
                    {/* Personal Information */}
                    <Card>
                        <CardHeader>
                            <CardTitle className="flex items-center">
                                <User className="h-5 w-5 mr-2" />
                                Thông tin đặt bàn
                            </CardTitle>
                        </CardHeader>
                        <CardContent className="space-y-4">
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                <div className="space-y-2">
                                    <Label htmlFor="customerName">
                                        Tên khách hàng <span className="text-red-500">*</span>
                                    </Label>
                                    <Input
                                        id="customerName"
                                        placeholder="Nhập tên của bạn"
                                        value={formData.customerName}
                                        onChange={(e) => handleInputChange('customerName', e.target.value)}
                                        required
                                    />
                                </div>

                                <div className="space-y-2">
                                    <Label htmlFor="phone">
                                        Số điện thoại <span className="text-red-500">*</span>
                                    </Label>
                                    <div className="flex">
                                        <span className="inline-flex items-center px-3 border border-r-0 border-gray-300 rounded-l-md bg-gray-50 text-gray-500">
                                            <Phone className="h-4 w-4" />
                                        </span>
                                        <Input
                                            id="phone"
                                            type="tel"
                                            placeholder="0987654321"
                                            value={formData.phone}
                                            onChange={(e) => handleInputChange('phone', e.target.value)}
                                            className="rounded-l-none"
                                            required
                                        />
                                    </div>
                                </div>

                                <div className="space-y-2">
                                    <Label htmlFor="date">
                                        Ngày đặt bàn <span className="text-red-500">*</span>
                                    </Label>
                                    <div className="flex">
                                        <span className="inline-flex items-center px-3 border border-r-0 border-gray-300 rounded-l-md bg-gray-50 text-gray-500">
                                            <Calendar className="h-4 w-4" />
                                        </span>
                                        <Input
                                            id="date"
                                            type="date"
                                            value={formData.date}
                                            onChange={(e) => handleInputChange('date', e.target.value)}
                                            min={new Date().toISOString().split('T')[0]}
                                            className="rounded-l-none"
                                            required
                                        />
                                    </div>
                                </div>

                                <div className="space-y-2">
                                    <Label htmlFor="time">
                                        Giờ đặt bàn <span className="text-red-500">*</span>
                                    </Label>
                                    <div className="flex">
                                        <span className="inline-flex items-center px-3 border border-r-0 border-gray-300 rounded-l-md bg-gray-50 text-gray-500">
                                            <Clock className="h-4 w-4" />
                                        </span>
                                        <Input
                                            id="time"
                                            type="time"
                                            value={formData.time}
                                            onChange={(e) => handleInputChange('time', e.target.value)}
                                            className="rounded-l-none"
                                            required
                                        />
                                    </div>
                                </div>

                                <div className="space-y-2">
                                    <Label htmlFor="guests">
                                        Số khách <span className="text-red-500">*</span>
                                    </Label>
                                    <div className="flex">
                                        <span className="inline-flex items-center px-3 border border-r-0 border-gray-300 rounded-l-md bg-gray-50 text-gray-500">
                                            <Users className="h-4 w-4" />
                                        </span>
                                        <Input
                                            id="guests"
                                            type="number"
                                            min="1"
                                            max="20"
                                            placeholder="3"
                                            value={formData.guests}
                                            onChange={(e) => handleInputChange('guests', e.target.value)}
                                            className="rounded-l-none"
                                            required
                                        />
                                    </div>
                                </div>

                                <div className="space-y-2">
                                    <Label>Bàn đã chọn</Label>
                                    <Button
                                        type="button"
                                        variant="outline"
                                        className="w-full justify-start"
                                        onClick={handleSelectTable}
                                    >
                                        <TableIcon className="h-4 w-4 mr-2" />
                                        {selectedTable ? `${selectedTable.name} - ${selectedTable.floor}` : 'Chọn bàn phù hợp'}
                                    </Button>
                                </div>
                            </div>

                            <div className="space-y-2">
                                <Label htmlFor="note">Ghi chú</Label>
                                <Textarea
                                    id="note"
                                    placeholder="Yêu cầu đặc biệt, dị ứng thực phẩm..."
                                    value={formData.note}
                                    onChange={(e) => handleInputChange('note', e.target.value)}
                                    rows={3}
                                />
                            </div>
                        </CardContent>
                    </Card>

                    {/* Submit Button */}
                    <div className="flex justify-end gap-4">
                        <Button
                            type="button"
                            variant="outline"
                            onClick={() => onPageChange('menu')}
                        >
                            Quay lại menu
                        </Button>
                        <Button
                            type="submit"
                            disabled={loading || !selectedTable}
                            className="min-w-[200px]"
                        >
                            {loading ? (
                                <>
                                    <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                                    Đang xử lý...
                                </>
                            ) : (
                                'Xác nhận đặt bàn'
                            )}
                        </Button>
                    </div>
                </div>

                {/* Order Summary Column */}
                <div className="lg:col-span-1">
                    <Card className="sticky top-4">
                        <CardHeader>
                            <CardTitle>Chi tiết đơn hàng</CardTitle>
                        </CardHeader>
                        <CardContent className="space-y-4">
                            {cartItems.length === 0 ? (
                                <p className="text-gray-600 text-sm">Chưa có món ăn nào</p>
                            ) : (
                                <>
                                    <div className="space-y-3 max-h-[300px] overflow-y-auto">
                                        {cartItems.map((item) => (
                                            <div key={item.menuItem.id} className="flex justify-between items-center text-sm">
                                                <div className="flex-1">
                                                    <div className="font-medium">{item.menuItem.name}</div>
                                                    <div className="text-gray-600">
                                                        {item.menuItem.price.toLocaleString('vi-VN')}đ x {item.quantity}
                                                    </div>
                                                </div>
                                                <div className="font-medium">
                                                    {(item.menuItem.price * item.quantity).toLocaleString('vi-VN')}đ
                                                </div>
                                            </div>
                                        ))}
                                    </div>
                                    <div className="border-t pt-3">
                                        <div className="flex justify-between font-bold text-lg">
                                            <span>Tổng cộng:</span>
                                            <span className="text-primary">
                                                {getTotalPrice().toLocaleString('vi-VN')}đ
                                            </span>
                                        </div>
                                    </div>
                                </>
                            )}
                        </CardContent>
                    </Card>
                </div>
            </form>

            {/* FLOOR SELECTION MODAL - REDESIGNED */}
            {showFloorSelection && (
                <div className="fixed inset-0 bg-black bg-opacity-60 flex items-center justify-center z-50 p-4 animate-fadeIn">
                    <Card className="w-full max-w-5xl max-h-[90vh] overflow-hidden shadow-2xl animate-slideUp">
                        <CardHeader className="bg-gradient-to-r from-blue-600 to-purple-600 text-white">
                            <div className="flex items-center justify-between">
                                <div>
                                    <CardTitle className="flex items-center text-2xl">
                                        <MapPin className="h-6 w-6 mr-3" />
                                        Chọn Tầng
                                    </CardTitle>
                                    <p className="text-blue-100 text-sm mt-1">
                                        Hệ thống gợi ý tầng phù hợp với số khách của bạn
                                    </p>
                                </div>
                                <Button
                                    variant="ghost"
                                    size="sm"
                                    onClick={() => setShowFloorSelection(false)}
                                    className="text-white hover:bg-white hover:bg-opacity-20"
                                >
                                    <XCircle className="h-5 w-5" />
                                </Button>
                            </div>
                        </CardHeader>
                        <CardContent className="p-8">
                            {/* Guest Info Banner */}
                            <div className="mb-8 p-5 bg-gradient-to-r from-blue-50 to-indigo-50 rounded-xl border-2 border-blue-200">
                                <div className="flex items-center justify-center gap-6">
                                    <div className="flex items-center text-blue-800">
                                        <Users className="h-6 w-6 mr-2 text-blue-600" />
                                        <div>
                                            <p className="text-sm text-blue-600 font-medium">Số khách</p>
                                            <p className="text-2xl font-bold">{formData.guests} người</p>
                                        </div>
                                    </div>
                                    <div className="h-12 w-px bg-blue-300"></div>
                                    <div className="flex items-center text-blue-800">
                                        <Calendar className="h-6 w-6 mr-2 text-blue-600" />
                                        <div>
                                            <p className="text-sm text-blue-600 font-medium">Ngày & Giờ</p>
                                            <p className="text-lg font-semibold">{formData.date} • {formData.time}</p>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            {/* Floor Cards Grid */}
                            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                                {floors.map((floor) => (
                                    <div
                                        key={floor.name}
                                        className={`
                      relative p-6 rounded-2xl border-3 cursor-pointer transition-all duration-300 transform
                      ${floor.disabled
                                                ? 'border-gray-300 bg-gray-50 opacity-50 cursor-not-allowed'
                                                : `border-${floor.color}-200 ${floor.bgColor} ${floor.hoverColor} hover:scale-105 hover:shadow-xl`
                                            }
                      ${floor.recommended && !floor.disabled ? 'ring-4 ring-amber-400 ring-opacity-50 shadow-lg' : ''}
                    `}
                                        onClick={() => handleSelectFloor(floor.name, floor.disabled)}
                                    >
                                        {/* Recommended Badge */}
                                        {floor.recommended && !floor.disabled && (
                                            <div className="absolute -top-3 -right-3 bg-gradient-to-r from-amber-400 to-orange-500 text-white px-4 py-1 rounded-full text-xs font-bold shadow-lg flex items-center gap-1 animate-pulse">
                                                <Star className="h-3 w-3 fill-white" />
                                                Khuyến nghị
                                            </div>
                                        )}

                                        {/* Disabled Badge */}
                                        {floor.disabled && (
                                            <div className="absolute -top-3 -right-3 bg-gray-400 text-white px-4 py-1 rounded-full text-xs font-bold shadow-lg">
                                                Không phù hợp
                                            </div>
                                        )}

                                        <div className="text-center">
                                            {/* Icon Circle */}
                                            <div className={`
                        w-24 h-24 mx-auto mb-4 rounded-full flex items-center justify-center text-5xl
                        ${floor.disabled ? 'bg-gray-200' : `bg-${floor.color}-100 shadow-lg`}
                        transition-transform duration-300 hover:rotate-12
                      `}>
                                                {floor.icon}
                                            </div>

                                            {/* Floor Name */}
                                            <h3 className={`
                        text-2xl font-bold mb-3
                        ${floor.disabled ? 'text-gray-500' : `text-${floor.color}-700`}
                      `}>
                                                {floor.name}
                                            </h3>

                                            {/* Tables Count */}
                                            <div className={`
                        inline-flex items-center gap-2 px-4 py-2 rounded-full mb-3
                        ${floor.disabled
                                                    ? 'bg-gray-200 text-gray-600'
                                                    : `bg-${floor.color}-200 text-${floor.color}-800`
                                                }
                      `}>
                                                <TableIcon className="h-4 w-4" />
                                                <span className="font-bold text-lg">{floor.tables} bàn</span>
                                                <CheckCircle2 className="h-4 w-4" />
                                            </div>

                                            {/* Capacity */}
                                            <p className={`
                        text-sm font-medium
                        ${floor.disabled ? 'text-gray-500' : `text-${floor.color}-600`}
                      `}>
                                                Phù hợp {floor.capacity}
                                            </p>

                                            {/* Status Indicator */}
                                            <div className="mt-4 pt-4 border-t border-opacity-30">
                                                {floor.disabled ? (
                                                    <p className="text-xs text-gray-500 flex items-center justify-center gap-1">
                                                        <XCircle className="h-3 w-3" />
                                                        Quá sức chứa
                                                    </p>
                                                ) : (
                                                    <p className={`text-xs text-${floor.color}-600 flex items-center justify-center gap-1 font-semibold`}>
                                                        <CheckCircle2 className="h-3 w-3" />
                                                        Sẵn sàng phục vụ
                                                    </p>
                                                )}
                                            </div>
                                        </div>
                                    </div>
                                ))}
                            </div>

                            {/* Help Text */}
                            <div className="mt-8 text-center">
                                <p className="text-sm text-gray-600">
                                    💡 <strong>Gợi ý:</strong> Chọn tầng được đánh dấu "Khuyến nghị" để có trải nghiệm tốt nhất
                                </p>
                            </div>
                        </CardContent>
                    </Card>
                </div>
            )}

            {/* TABLE SELECTION MODAL (Keep existing code) */}
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

            <style jsx>{`
        @keyframes fadeIn {
          from { opacity: 0; }
          to { opacity: 1; }
        }
        @keyframes slideUp {
          from { 
            opacity: 0;
            transform: translateY(20px);
          }
          to { 
            opacity: 1;
            transform: translateY(0);
          }
        }
        .animate-fadeIn {
          animation: fadeIn 0.3s ease-out;
        }
        .animate-slideUp {
          animation: slideUp 0.4s ease-out;
        }
      `}</style>
        </div>
    );
};

