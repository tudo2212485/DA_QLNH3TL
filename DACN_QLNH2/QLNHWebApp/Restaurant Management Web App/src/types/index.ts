export interface MenuItem {
  id: number;
  name: string;
  description: string;
  price: number;
  category: string;
  imageUrl: string;
  ratings?: Rating[];
}

export interface Rating {
  id: number;
  menuItemId: number;
  score: number;
  comment: string;
  date: string;
}

export interface CartItem {
  menuItem: MenuItem;
  quantity: number;
}

export interface Order {
  id: number;
  customerName: string;
  phone: string;
  date: string;
  time: string;
  guests: number;
  note?: string;
  totalPrice: number;
  status: string;
  orderItems: OrderItem[];
}

export interface OrderItem {
  id: number;
  orderId: number;
  menuItemId: number;
  menuItem: MenuItem;
  quantity: number;
  price: number;
}

export interface ContactMessage {
  id: number;
  name: string;
  email: string;
  message: string;
  date: string;
}

export type MenuCategory = 'khai-vi' | 'mon-chinh' | 'mon-nuong' | 'lau' | 'trang-mieng' | 'do-uong';

export const MENU_CATEGORIES: { value: MenuCategory; label: string }[] = [
  { value: 'khai-vi', label: 'Món khai vị' },
  { value: 'mon-chinh', label: 'Món chính' },
  { value: 'mon-nuong', label: 'Món nướng' },
  { value: 'lau', label: 'Các món lẩu' },
  { value: 'trang-mieng', label: 'Tráng miệng' },
  { value: 'do-uong', label: 'Đồ uống' },
];