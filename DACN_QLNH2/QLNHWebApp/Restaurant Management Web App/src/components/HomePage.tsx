import React, { useState, useEffect } from 'react';
import { Button } from './ui/button';
import { Card, CardContent } from './ui/card';
import { Badge } from './ui/badge';
import { Star, Clock, Award, Users } from 'lucide-react';
import { ImageWithFallback } from './figma/ImageWithFallback';
import { MenuItem } from '../types/index';

interface HomePageProps {
  onPageChange: (page: string) => void;
}

export const HomePage: React.FC<HomePageProps> = ({ onPageChange }) => {
  const [featuredItems, setFeaturedItems] = useState<MenuItem[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchFeaturedItems = async () => {
      try {
        const response = await fetch('/api/MenuApi');
        if (response.ok) {
          const items: MenuItem[] = await response.json();
          // Lấy 6 món đầu tiên làm featured items
          setFeaturedItems(items.slice(0, 6));
        }
      } catch (error) {
        console.error('Lỗi khi tải món ăn:', error);
        // Fallback to empty array if API fails
        setFeaturedItems([]);
      } finally {
        setLoading(false);
      }
    };

    fetchFeaturedItems();
  }, []);

  return (
    <div className="space-y-16">
      {/* Hero Section */}
      <section className="relative bg-gradient-to-r from-primary/10 to-primary/5 py-20">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-12 items-center">
            <div className="space-y-6">
              <h1 className="text-4xl lg:text-6xl font-bold text-gray-900">
                Hương vị 
                <span className="text-primary"> truyền thống</span> 
                <br />Việt Nam
              </h1>
              <p className="text-xl text-gray-600">
                Thưởng thức những món ăn đặc sắc với hương vị đậm đà, 
                được chế biến từ những nguyên liệu tươi ngon nhất.
              </p>
              <div className="flex flex-col sm:flex-row gap-4">
                <Button 
                  size="lg" 
                  onClick={() => onPageChange('menu')}
                  className="text-lg px-8 py-6"
                >
                  Xem Thực Đơn
                </Button>
                <Button 
                  variant="outline" 
                  size="lg"
                  onClick={() => onPageChange('booking')}
                  className="text-lg px-8 py-6"
                >
                  Đặt Bàn Ngay
                </Button>
              </div>
            </div>
            <div className="relative">
              <ImageWithFallback
                src="https://images.unsplash.com/photo-1667388968964-4aa652df0a9b?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxyZXN0YXVyYW50JTIwaW50ZXJpb3IlMjBkaW5pbmd8ZW58MXx8fHwxNzU4MTk4ODQwfDA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral"
                alt="Nhà hàng 3TL"
                className="rounded-2xl shadow-2xl w-full h-[400px] object-cover"
              />
            </div>
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section className="py-16 bg-gray-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-12">
            <h2 className="text-3xl font-bold text-gray-900 mb-4">
              Tại sao chọn Nhà Hàng 3TL?
            </h2>
            <p className="text-lg text-gray-600 max-w-2xl mx-auto">
              Chúng tôi cam kết mang đến cho bạn trải nghiệm ẩm thực tuyệt vời nhất
            </p>
          </div>
          
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            <div className="text-center">
              <div className="bg-primary/10 rounded-full w-16 h-16 flex items-center justify-center mx-auto mb-4">
                <Award className="h-8 w-8 text-primary" />
              </div>
              <h3 className="text-xl font-semibold mb-2">Menu phong phú</h3>
              <p className="text-gray-600">
                Đa dạng các món ăn từ khai vị đến tráng miệng, 
                phù hợp với mọi khẩu vị
              </p>
            </div>
            
            <div className="text-center">
              <div className="bg-primary/10 rounded-full w-16 h-16 flex items-center justify-center mx-auto mb-4">
                <Users className="h-8 w-8 text-primary" />
              </div>
              <h3 className="text-xl font-semibold mb-2">Không gian rộng rãi</h3>
              <p className="text-gray-600">
                Thiết kế hiện đại, thoáng mát với sức chứa lên đến 200 khách
              </p>
            </div>
            
            <div className="text-center">
              <div className="bg-primary/10 rounded-full w-16 h-16 flex items-center justify-center mx-auto mb-4">
                <Clock className="h-8 w-8 text-primary" />
              </div>
              <h3 className="text-xl font-semibold mb-2">Phục vụ tận tình</h3>
              <p className="text-gray-600">
                Đội ngũ nhân viên chuyên nghiệp, nhiệt tình phục vụ 24/7
              </p>
            </div>
          </div>
        </div>
      </section>

      {/* Featured Menu */}
      <section className="py-16">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-12">
            <h2 className="text-3xl font-bold text-gray-900 mb-4">
              Món ăn nổi bật
            </h2>
            <p className="text-lg text-gray-600">
              Những món ăn được khách hàng yêu thích nhất
            </p>
          </div>
          
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
            {loading ? (
              // Loading skeleton
              Array.from({ length: 6 }).map((_, index) => (
                <Card key={index} className="overflow-hidden">
                  <div className="w-full h-48 bg-gray-200 animate-pulse" />
                  <CardContent className="p-4">
                    <div className="h-4 bg-gray-200 animate-pulse mb-2" />
                    <div className="h-3 bg-gray-200 animate-pulse mb-3" />
                    <div className="flex justify-between items-center">
                      <div className="h-4 w-20 bg-gray-200 animate-pulse" />
                      <div className="h-6 w-16 bg-gray-200 animate-pulse rounded-full" />
                    </div>
                  </CardContent>
                </Card>
              ))
            ) : featuredItems.length > 0 ? (
              featuredItems.map((item) => (
              <Card key={item.id} className="overflow-hidden hover:shadow-lg transition-shadow">
                <div className="relative">
                  <ImageWithFallback
                    src={item.imageUrl}
                    alt={item.name}
                    className="w-full h-48 object-cover"
                  />
                  {item.ratings && item.ratings.length > 0 && (
                    <Badge className="absolute top-3 right-3 bg-white text-gray-800">
                      <Star className="h-3 w-3 mr-1 fill-current text-yellow-500" />
                      {(item.ratings.reduce((sum, rating) => sum + rating.score, 0) / item.ratings.length).toFixed(1)}
                    </Badge>
                  )}
                </div>
                <CardContent className="p-4">
                  <h3 className="font-semibold mb-2">{item.name}</h3>
                  <p className="text-sm text-gray-600 mb-3">{item.description}</p>
                  <div className="flex justify-between items-center">
                    <span className="font-bold text-lg text-primary">
                      {item.price.toLocaleString('vi-VN')}đ
                    </span>
                    <Badge variant="secondary">
                      {item.category}
                    </Badge>
                  </div>
                </CardContent>
              </Card>
              ))
            ) : (
              // Empty state
              <div className="col-span-full text-center py-8">
                <p className="text-gray-500 text-lg">Đang cập nhật thực đơn...</p>
              </div>
            )}
          </div>
          
          <div className="text-center mt-12">
            <Button size="lg" onClick={() => onPageChange('menu')}>
              Xem tất cả món ăn
            </Button>
          </div>
        </div>
      </section>

      {/* Call to Action */}
      <section className="py-16 bg-primary text-white">
        <div className="max-w-4xl mx-auto text-center px-4 sm:px-6 lg:px-8">
          <h2 className="text-3xl font-bold mb-4">
            Sẵn sàng thưởng thức?
          </h2>
          <p className="text-xl mb-8 opacity-90">
            Đặt bàn ngay hôm nay để không bỏ lỡ những món ăn tuyệt vời
          </p>
          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            <Button 
              variant="secondary" 
              size="lg"
              onClick={() => onPageChange('booking')}
            >
              Đặt Bàn Ngay
            </Button>
            <Button 
              variant="outline" 
              size="lg"
              onClick={() => onPageChange('contact')}
              className="border-white text-white hover:bg-white hover:text-primary"
            >
              Liên Hệ
            </Button>
          </div>
        </div>
      </section>
    </div>
  );
};