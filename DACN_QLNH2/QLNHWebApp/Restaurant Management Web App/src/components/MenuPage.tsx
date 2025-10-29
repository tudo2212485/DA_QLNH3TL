import React, { useState, useMemo, useEffect } from 'react';
import { Button } from './ui/button';
import { Card, CardContent } from './ui/card';
import { Badge } from './ui/badge';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from './ui/dialog';
import { Star, Plus, Eye } from 'lucide-react';
import { ImageWithFallback } from './figma/ImageWithFallback';
import { MENU_CATEGORIES, MenuItem, MenuCategory } from '../types/index';
import { useCart } from '../hooks/useCart';
import { toast } from 'sonner@2.0.3';
import { apiService } from '../services/api';

interface MenuPageProps {
  searchQuery: string;
}

export const MenuPage: React.FC<MenuPageProps> = ({ searchQuery }) => {
  const [selectedCategory, setSelectedCategory] = useState<MenuCategory | 'all'>('all');
  const [selectedItem, setSelectedItem] = useState<MenuItem | null>(null);
  const [menuItems, setMenuItems] = useState<MenuItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const { addToCart } = useCart();

  useEffect(() => {
    const fetchMenuItems = async () => {
      try {
        setLoading(true);
        setError(null);

        // Map category values to match API
        const categoryMap: Record<string, string> = {
          'khai-vi': 'Món khai vị',
          'mon-chinh': 'Món chính',
          'mon-nuong': 'Món nướng',
          'lau': 'Các món lẩu',
          'trang-mieng': 'Tráng miệng',
          'do-uong': 'Đồ uống'
        };

        const apiCategory = selectedCategory === 'all' ? undefined : categoryMap[selectedCategory];
        const items = await apiService.getMenuItems(apiCategory, searchQuery || undefined);
        setMenuItems(items);
      } catch (err) {
        setError('Không thể tải thực đơn. Vui lòng thử lại sau.');
        console.error('Error fetching menu items:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchMenuItems();
  }, [selectedCategory, searchQuery]);

  const filteredItems = useMemo(() => {
    return menuItems;
  }, [menuItems]);

  const handleAddToCart = (item: MenuItem) => {
    addToCart(item);
    toast.success(`Đã thêm ${item.name} vào giỏ hàng`);
  };

  const handleViewDetails = (item: MenuItem) => {
    setSelectedItem(item);
  };

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      {/* Page Header */}
      <div className="text-center mb-8">
        <h1 className="text-4xl font-bold text-gray-900 mb-4">Thực đơn</h1>
        <p className="text-lg text-gray-600">
          Khám phá những món ăn đặc sắc của chúng tôi
        </p>
      </div>

      {/* Category Filter */}
      <div className="mb-8">
        <div className="flex flex-wrap gap-2 justify-center">
          <Button
            variant={selectedCategory === 'all' ? 'default' : 'outline'}
            onClick={() => setSelectedCategory('all')}
            className="mb-2"
          >
            Xem tất cả
          </Button>
          {MENU_CATEGORIES?.map((category) => (
            <Button
              key={category.value}
              variant={selectedCategory === category.value ? 'default' : 'outline'}
              onClick={() => setSelectedCategory(category.value)}
              className="mb-2"
            >
              {category.label}
            </Button>
          ))}
        </div>
      </div>

      {/* Search Results Info */}
      {searchQuery.trim() && (
        <div className="mb-6">
          <p className="text-gray-600">
            Tìm thấy {filteredItems.length} món ăn cho "{searchQuery}"
          </p>
        </div>
      )}

      {/* Loading State */}
      {loading && (
        <div className="text-center py-12">
          <p className="text-gray-500 text-lg">Đang tải thực đơn...</p>
        </div>
      )}

      {/* Error State */}
      {error && (
        <div className="text-center py-12">
          <p className="text-red-500 text-lg">{error}</p>
          <Button
            onClick={() => window.location.reload()}
            className="mt-4"
          >
            Thử lại
          </Button>
        </div>
      )}

      {/* Menu Items Grid */}
      {!loading && !error && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
          {filteredItems.map((item) => (
            <Card key={item.id} className="overflow-hidden hover:shadow-lg transition-shadow">
              <div className="relative">
                <ImageWithFallback
                  src={item.imageUrl}
                  alt={item.name}
                  className="w-full h-48 object-cover"
                />
                {item.ratings && item.ratings.length > 0 && (
                  <Badge className="absolute top-3 left-3 bg-white text-gray-800">
                    <Star className="h-3 w-3 mr-1 fill-current text-yellow-500" />
                    {(item.ratings.reduce((sum, r) => sum + r.score, 0) / item.ratings.length).toFixed(1)}
                  </Badge>
                )}
                <Badge
                  variant="secondary"
                  className="absolute top-3 right-3"
                >
                  {item.category}
                </Badge>
              </div>

              <CardContent className="p-4">
                <h3 className="font-semibold mb-2">{item.name}</h3>
                <p className="text-sm text-gray-600 mb-3 line-clamp-2">
                  {item.description}
                </p>

                <div className="flex justify-between items-center mb-4">
                  <span className="font-bold text-lg text-primary">
                    {item.price.toLocaleString('vi-VN')}đ
                  </span>
                </div>

                <div className="flex gap-2">
                  <Button
                    size="sm"
                    onClick={() => handleAddToCart(item)}
                    className="flex-1"
                  >
                    <Plus className="h-4 w-4 mr-1" />
                    Đặt món
                  </Button>
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => handleViewDetails(item)}
                  >
                    <Eye className="h-4 w-4" />
                  </Button>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      )}

      {/* No Results */}
      {!loading && !error && filteredItems.length === 0 && (
        <div className="text-center py-12">
          <p className="text-gray-500 text-lg">
            Không tìm thấy món ăn nào phù hợp
          </p>
        </div>
      )}

      {/* Item Details Modal */}
      <Dialog open={!!selectedItem} onOpenChange={() => setSelectedItem(null)}>
        <DialogContent className="max-w-2xl">
          {selectedItem && (
            <>
              <DialogHeader>
                <DialogTitle>{selectedItem.name}</DialogTitle>
              </DialogHeader>

              <div className="space-y-4">
                <ImageWithFallback
                  src={selectedItem.imageUrl}
                  alt={selectedItem.name}
                  className="w-full h-64 object-cover rounded-lg"
                />

                <div className="flex justify-between items-center">
                  <Badge variant="secondary">
                    {selectedItem.category}
                  </Badge>
                  {selectedItem.ratings && selectedItem.ratings.length > 0 && (
                    <div className="flex items-center">
                      <Star className="h-4 w-4 mr-1 fill-current text-yellow-500" />
                      <span>{(selectedItem.ratings.reduce((sum, r) => sum + r.score, 0) / selectedItem.ratings.length).toFixed(1)}/5</span>
                    </div>
                  )}
                </div>

                <p className="text-gray-600">{selectedItem.description}</p>

                <div className="flex justify-between items-center">
                  <span className="text-2xl font-bold text-primary">
                    {selectedItem.price.toLocaleString('vi-VN')}đ
                  </span>
                  <Button onClick={() => handleAddToCart(selectedItem)}>
                    <Plus className="h-4 w-4 mr-2" />
                    Thêm vào giỏ hàng
                  </Button>
                </div>

                {/* Reviews */}
                {selectedItem.ratings && selectedItem.ratings.length > 0 && (
                  <div className="space-y-4">
                    <h4 className="font-semibold">Đánh giá từ khách hàng</h4>
                    <div className="space-y-3">
                      {selectedItem.ratings.map((rating) => (
                        <div key={rating.id} className="border rounded-lg p-3">
                          <div className="flex justify-between items-center mb-2">
                            <span className="font-medium">Khách hàng</span>
                            <div className="flex items-center">
                              <Star className="h-4 w-4 mr-1 fill-current text-yellow-500" />
                              <span>{rating.score}/5</span>
                            </div>
                          </div>
                          <p className="text-sm text-gray-600">{rating.comment}</p>
                          <p className="text-xs text-gray-400 mt-1">{new Date(rating.date).toLocaleDateString('vi-VN')}</p>
                        </div>
                      ))}
                    </div>
                  </div>
                )}
              </div>
            </>
          )}
        </DialogContent>
      </Dialog>
    </div>
  );
};