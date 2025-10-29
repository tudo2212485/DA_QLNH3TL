import { MenuItem, Review } from '../types/index';

export const mockReviews: Review[] = [
  {
    id: '1',
    customerName: 'Nguyễn Văn A',
    rating: 5,
    comment: 'Món ăn rất ngon, phục vụ tận tình!',
    date: '2024-01-15'
  },
  {
    id: '2',
    customerName: 'Trần Thị B',
    rating: 4,
    comment: 'Không gian đẹp, vị ngon.',
    date: '2024-01-10'
  }
];

export const mockMenuItems: MenuItem[] = [
  // Món khai vị
  {
    id: '1',
    name: 'Gỏi cuốn tôm thịt',
    description: 'Gỏi cuốn tươi ngon với tôm, thịt, rau sống và bún tươi',
    price: 45000,
    category: 'khai-vi',
    imageUrl: 'https://images.unsplash.com/photo-1693494869603-09f1981f28e0?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwc3ByaW5nJTIwcm9sbHN8ZW58MXx8fHwxNzU4MjM1ODU2fDA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
    rating: 4.5,
    reviews: mockReviews
  },
  {
    id: '2',
    name: 'Nem nướng Nha Trang',
    description: 'Nem nướng thơm ngon đặc sản Nha Trang',
    price: 65000,
    category: 'khai-vi',
    imageUrl: 'https://images.unsplash.com/photo-1723744895523-75a5b52de0eb?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxyZXN0YXVyYW50JTIwZm9vZCUyMGRpc2hlc3xlbnwxfHx8fDE3NTgyNDc5MzV8MA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
    rating: 4.2
  },
  {
    id: '3',
    name: 'Salad bò khô',
    description: 'Salad tươi mát với bò khô thái sợi',
    price: 85000,
    category: 'khai-vi',
    imageUrl: 'https://images.unsplash.com/photo-1723744895523-75a5b52de0eb?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxyZXN0YXVyYW50JTIwZm9vZCUyMGRpc2hlc3xlbnwxfHx8fDE3NTgyNDc5MzV8MA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
    rating: 4.0
  },

  // Món chính
  {
    id: '4',
    name: 'Phở bò đặc biệt',
    description: 'Phở bò truyền thống với nước dầm đậm đà, thịt bò tái và chín',
    price: 75000,
    category: 'mon-chinh',
    imageUrl: 'https://images.unsplash.com/photo-1631709497146-a239ef373cf1?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwcGhvJTIwbm9vZGxlc3xlbnwxfHx8fDE3NTgyNDc5NDB8MA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
    rating: 4.8
  },
  {
    id: '5',
    name: 'Cơm tấm sườn nướng',
    description: 'Cơm tấm với sườn nướng thơm lừng, chả trứng và bì',
    price: 65000,
    category: 'mon-chinh',
    imageUrl: 'https://images.unsplash.com/photo-1723744895523-75a5b52de0eb?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxyZXN0YXVyYW50JTIwZm9vZCUyMGRpc2hlc3xlbnwxfHx8fDE3NTgyNDc5MzV8MA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
    rating: 4.3
  },
  {
    id: '6',
    name: 'Bún bò Huế',
    description: 'Bún bò Huế cay nồng đặc trưng miền Trung',
    price: 70000,
    category: 'mon-chinh',
    imageUrl: 'https://images.unsplash.com/photo-1631709497146-a239ef373cf1?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwcGhvJTIwbm9vZGxlc3xlbnwxfHx8fDE3NTgyNDc5NDB8MA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
    rating: 4.6
  },

  // Món nướng
  {
    id: '7',
    name: 'Thịt nướng xiên que',
    description: 'Thịt nướng xiên que thơm ngon, ướp gia vị đậm đà',
    price: 120000,
    category: 'mon-nuong',
    imageUrl: 'https://images.unsplash.com/photo-1702741168115-cd3d9a682972?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxncmlsbGVkJTIwbWVhdCUyMGJhcmJlY3VlfGVufDF8fHx8MTc1ODI0Nzk0Mnww&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
    rating: 4.4
  },
  {
    id: '8',
    name: 'Cá nướng lá chuối',
    description: 'Cá nướng trong lá chuối giữ nguyên hương vị tự nhiên',
    price: 150000,
    category: 'mon-nuong',
    imageUrl: 'https://images.unsplash.com/photo-1702741168115-cd3d9a682972?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxncmlsbGVkJTIwbWVhdCUyMGJhcmJlY3VlfGVufDF8fHx8MTc1ODI0Nzk0Mnww&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
    rating: 4.7
  },
  {
    id: '9',
    name: 'Gà nướng mật ong',
    description: 'Gà nướng với mật ong, da vàng giòn, thịt mềm ngọt',
    price: 180000,
    category: 'mon-nuong',
    imageUrl: 'https://images.unsplash.com/photo-1702741168115-cd3d9a682972?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxncmlsbGVkJTIwbWVhdCUyMGJhcmJlY3VlfGVufDF8fHx8MTc1ODI0Nzk0Mnww&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
    rating: 4.5
  },

  // Các món lẩu
  {
    id: '10',
    name: 'Lẩu thái hải sản',
    description: 'Lẩu thái chua cay với hải sản tươi ngon',
    price: 350000,
    category: 'lau',
    imageUrl: 'https://images.unsplash.com/photo-1723744895523-75a5b52de0eb?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxyZXN0YXVyYW50JTIwZm9vZCUyMGRpc2hlc3xlbnwxfHx8fDE3NTgyNDc5MzV8MA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
    rating: 4.6
  },
  {
    id: '11',
    name: 'Lẩu gà lá é',
    description: 'Lẩu gà truyền thống với lá é thơm đặc trưng',
    price: 280000,
    category: 'lau',
    imageUrl: 'https://images.unsplash.com/photo-1723744895523-75a5b52de0eb?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxyZXN0YXVyYW50JTIwZm9vZCUyMGRpc2hlc3xlbnwxfHx8fDE3NTgyNDc5MzV8MA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
    rating: 4.3
  },

  // Tráng miệng
  {
    id: '12',
    name: 'Chè đậu đỏ',
    description: 'Chè đậu đỏ truyền thống, ngọt mát',
    price: 25000,
    category: 'trang-mieng',
    imageUrl: 'https://images.unsplash.com/photo-1586727579295-62136fbb6082?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwZGVzc2VydHxlbnwxfHx8fDE3NTgyNDc5NDd8MA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
    rating: 4.1
  },
  {
    id: '13',
    name: 'Bánh flan',
    description: 'Bánh flan mịn màng, vị caramel thơm ngon',
    price: 30000,
    category: 'trang-mieng',
    imageUrl: 'https://images.unsplash.com/photo-1586727579295-62136fbb6082?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwZGVzc2VydHxlbnwxfHx8fDE3NTgyNDc5NDd8MA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
    rating: 4.4
  },
  {
    id: '14',
    name: 'Kem xôi dừa',
    description: 'Kem xôi dừa mát lạnh, thơm ngon',
    price: 35000,
    category: 'trang-mieng',
    imageUrl: 'https://images.unsplash.com/photo-1586727579295-62136fbb6082?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwZGVzc2VydHxlbnwxfHx8fDE3NTgyNDc5NDd8MA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
    rating: 4.2
  },

  // Đồ uống
  {
    id: '15',
    name: 'Trà đá',
    description: 'Trà đá truyền thống, mát lạnh',
    price: 10000,
    category: 'do-uong',
    imageUrl: 'https://images.unsplash.com/photo-1723744895523-75a5b52de0eb?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxyZXN0YXVyYW50JTIwZm9vZCUyMGRpc2hlc3xlbnwxfHx8fDE3NTgyNDc5MzV8MA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
    rating: 3.8
  },
  {
    id: '16',
    name: 'Nước chanh dây',
    description: 'Nước chanh dây tươi, chua ngọt mát lạnh',
    price: 25000,
    category: 'do-uong',
    imageUrl: 'https://images.unsplash.com/photo-1723744895523-75a5b52de0eb?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxyZXN0YXVyYW50JTIwZm9vZCUyMGRpc2hlc3xlbnwxfHx8fDE3NTgyNDc5MzV8MA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
    rating: 4.0
  },
  {
    id: '17',
    name: 'Cà phê sữa đá',
    description: 'Cà phê sữa đá đậm đà theo phong cách Việt Nam',
    price: 20000,
    category: 'do-uong',
    imageUrl: 'https://images.unsplash.com/photo-1723744895523-75a5b52de0eb?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxyZXN0YXVyYW50JTIwZm9vZCUyMGRpc2hlc3xlbnwxfHx8fDE3NTgyNDc5MzV8MA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
    rating: 4.5
  },
  {
    id: '18',
    name: 'Sinh tố bơ',
    description: 'Sinh tố bơ béo ngậy, thơm ngon',
    price: 35000,
    category: 'do-uong',
    imageUrl: 'https://images.unsplash.com/photo-1723744895523-75a5b52de0eb?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxyZXN0YXVyYW50JTIwZm9vZCUyMGRpc2hlc3xlbnwxfHx8fDE3NTgyNDc5MzV8MA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
    rating: 4.3
  }
];