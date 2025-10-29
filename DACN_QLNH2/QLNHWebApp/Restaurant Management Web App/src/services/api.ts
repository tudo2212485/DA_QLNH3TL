const API_BASE_URL = '/api';

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

export interface BookingRequest {
    customerName: string;
    phone: string;
    date: string;
    time: string;
    guests: number;
    note?: string;
}

export interface CheckoutRequest {
    customerName: string;
    phone: string;
    date: string;
    time: string;
    guests: number;
    note?: string;
    paymentMethod: string;
    tableId?: number;
    items: Array<{
        menuItemId: number;
        quantity: number;
    }>;
}

export interface ContactMessageRequest {
    name: string;
    email: string;
    message: string;
}

export interface RatingRequest {
    score: number;
    comment?: string;
}

class ApiService {
    private async request<T>(endpoint: string, options: RequestInit = {}): Promise<T> {
        const url = `${API_BASE_URL}${endpoint}`;
        const response = await fetch(url, {
            headers: {
                'Content-Type': 'application/json',
                ...options.headers,
            },
            ...options,
        });

        if (!response.ok) {
            throw new Error(`API request failed: ${response.statusText}`);
        }

        return response.json();
    }

    // Menu API
    async getMenuItems(category?: string, search?: string): Promise<MenuItem[]> {
        const params = new URLSearchParams();
        if (category) params.append('category', category);
        if (search) params.append('search', search);

        const queryString = params.toString();
        return this.request<MenuItem[]>(`/menuapi${queryString ? `?${queryString}` : ''}`);
    }

    async getMenuItem(id: number): Promise<MenuItem> {
        return this.request<MenuItem>(`/menuapi/${id}`);
    }

    async getCategories(): Promise<string[]> {
        return this.request<string[]>('/menuapi/categories');
    }

    async addRating(menuItemId: number, rating: RatingRequest): Promise<void> {
        await this.request(`/menuapi/${menuItemId}/ratings`, {
            method: 'POST',
            body: JSON.stringify(rating),
        });
    }

    // Order API
    async saveBookingInfo(booking: BookingRequest): Promise<void> {
        await this.request('/orderapi/save-booking-info', {
            method: 'POST',
            body: JSON.stringify(booking),
        });
    }

    async createBooking(booking: BookingRequest): Promise<Order> {
        return this.request<Order>('/orderapi/booking', {
            method: 'POST',
            body: JSON.stringify(booking),
        });
    }

    async createOrder(order: CheckoutRequest): Promise<Order> {
        return this.request<Order>('/orderapi/checkout', {
            method: 'POST',
            body: JSON.stringify(order),
        });
    }

    async getOrder(id: number): Promise<Order> {
        return this.request<Order>(`/orderapi/${id}`);
    }

    async getOrders(): Promise<Order[]> {
        return this.request<Order[]>('/orderapi');
    }

    // Contact API
    async createContactMessage(message: ContactMessageRequest): Promise<ContactMessage> {
        return this.request<ContactMessage>('/contactapi', {
            method: 'POST',
            body: JSON.stringify(message),
        });
    }

    async getContactMessages(): Promise<ContactMessage[]> {
        return this.request<ContactMessage[]>('/contactapi');
    }
}

export const apiService = new ApiService();
