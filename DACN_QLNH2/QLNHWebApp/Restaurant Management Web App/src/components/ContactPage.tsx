import React, { useState } from 'react';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Textarea } from './ui/textarea';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { MapPin, Phone, Mail, Clock, Send } from 'lucide-react';
import { ContactMessage } from '../types/index';
import { toast } from 'sonner@2.0.3';
import { apiService } from '../services/api';

export const ContactPage: React.FC = () => {
  const [formData, setFormData] = useState({
    name: '',
    email: '',
    message: ''
  });
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleInputChange = (field: string, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!formData.name || !formData.email || !formData.message) {
      toast.error('Vui lòng điền đầy đủ thông tin');
      return;
    }

    setIsSubmitting(true);

    try {
      // Create contact message via API
      await apiService.createContactMessage({
        name: formData.name,
        email: formData.email,
        message: formData.message
      });

      // Reset form
      setFormData({ name: '', email: '', message: '' });

      toast.success('Cảm ơn bạn đã liên hệ! Chúng tôi sẽ phản hồi sớm nhất có thể.');

    } catch (error) {
      console.error('Error creating contact message:', error);
      toast.error('Có lỗi xảy ra. Vui lòng thử lại sau.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const contactInfo = [
    {
      icon: <MapPin className="h-6 w-6" />,
      title: "Địa chỉ",
      content: "123 Đường Nguyễn Huệ, Quận 1, TP.HCM",
      details: "Gần chợ Bến Thành, dễ dàng di chuyển"
    },
    {
      icon: <Phone className="h-6 w-6" />,
      title: "Số điện thoại",
      content: "(028) 3822 3456",
      details: "Phục vụ 24/7 để hỗ trợ khách hàng"
    },
    {
      icon: <Mail className="h-6 w-6" />,
      title: "Email",
      content: "info@nhahang-saigon.com",
      details: "Phản hồi trong vòng 24 giờ"
    },
    {
      icon: <Clock className="h-6 w-6" />,
      title: "Giờ mở cửa",
      content: "10:00 - 22:00 (Hàng ngày)",
      details: "Chủ nhật: 9:00 - 23:00"
    }
  ];

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      {/* Header */}
      <div className="text-center mb-12">
        <h1 className="text-4xl font-bold text-gray-900 mb-4">
          Liên hệ với chúng tôi
        </h1>
        <p className="text-lg text-gray-600 max-w-2xl mx-auto">
          Chúng tôi rất mong được nghe ý kiến đóng góp từ bạn.
          Hãy liên hệ với chúng tôi qua form dưới đây hoặc thông tin liên hệ.
        </p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-12">
        {/* Contact Form */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center">
              <Send className="h-5 w-5 mr-2" />
              Gửi tin nhắn
            </CardTitle>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="space-y-6">
              <div className="space-y-2">
                <Label htmlFor="name">
                  Họ và tên <span className="text-red-500">*</span>
                </Label>
                <Input
                  id="name"
                  type="text"
                  value={formData.name}
                  onChange={(e) => handleInputChange('name', e.target.value)}
                  placeholder="Nhập họ và tên của bạn"
                  required
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="email">
                  Email <span className="text-red-500">*</span>
                </Label>
                <Input
                  id="email"
                  type="email"
                  value={formData.email}
                  onChange={(e) => handleInputChange('email', e.target.value)}
                  placeholder="Nhập địa chỉ email"
                  required
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="message">
                  Nội dung <span className="text-red-500">*</span>
                </Label>
                <Textarea
                  id="message"
                  value={formData.message}
                  onChange={(e) => handleInputChange('message', e.target.value)}
                  placeholder="Nhập nội dung tin nhắn..."
                  rows={6}
                  required
                />
              </div>

              <Button
                type="submit"
                className="w-full"
                size="lg"
                disabled={isSubmitting}
              >
                {isSubmitting ? (
                  <>
                    <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                    Đang gửi...
                  </>
                ) : (
                  <>
                    <Send className="h-4 w-4 mr-2" />
                    Gửi tin nhắn
                  </>
                )}
              </Button>
            </form>
          </CardContent>
        </Card>

        {/* Contact Information */}
        <div className="space-y-6">
          {contactInfo.map((info, index) => (
            <Card key={index} className="hover:shadow-md transition-shadow">
              <CardContent className="p-6">
                <div className="flex items-start space-x-4">
                  <div className="bg-primary/10 rounded-full p-3 text-primary">
                    {info.icon}
                  </div>
                  <div className="flex-1">
                    <h3 className="font-semibold text-lg mb-1">
                      {info.title}
                    </h3>
                    <p className="font-medium text-gray-900 mb-1">
                      {info.content}
                    </p>
                    <p className="text-sm text-gray-600">
                      {info.details}
                    </p>
                  </div>
                </div>
              </CardContent>
            </Card>
          ))}

          {/* Map Placeholder */}
          <Card>
            <CardContent className="p-6">
              <h3 className="font-semibold text-lg mb-4">Vị trí</h3>
              <div className="bg-gray-100 rounded-lg h-64 flex items-center justify-center">
                <div className="text-center">
                  <MapPin className="h-12 w-12 text-gray-400 mx-auto mb-2" />
                  <p className="text-gray-600">Bản đồ nhà hàng</p>
                  <p className="text-sm text-gray-500">
                    123 Đường Nguyễn Huệ, Quận 1, TP.HCM
                  </p>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Additional Info */}
          <Card className="bg-primary/5">
            <CardContent className="p-6">
              <h3 className="font-semibold text-lg mb-4">Thông tin thêm</h3>
              <div className="space-y-3 text-sm">
                <div className="flex justify-between">
                  <span className="text-gray-600">Sức chứa:</span>
                  <span className="font-medium">200 khách</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Bãi đỗ xe:</span>
                  <span className="font-medium">Miễn phí</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">WiFi:</span>
                  <span className="font-medium">Miễn phí</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Điều hòa:</span>
                  <span className="font-medium">Có</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Phục vụ nhóm:</span>
                  <span className="font-medium">Từ 10 người trở lên</span>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>

      {/* FAQ Section */}
      <section className="mt-16">
        <div className="text-center mb-8">
          <h2 className="text-3xl font-bold text-gray-900 mb-4">
            Câu hỏi thường gặp
          </h2>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <Card>
            <CardContent className="p-6">
              <h3 className="font-semibold mb-2">
                Nhà hàng có phục vụ đặt tiệc không?
              </h3>
              <p className="text-gray-600 text-sm">
                Có, chúng tôi nhận đặt tiệc cho các nhóm từ 10 người trở lên.
                Vui lòng liên hệ trước ít nhất 3 ngày để được tư vấn menu và giá cả.
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardContent className="p-6">
              <h3 className="font-semibold mb-2">
                Có dịch vụ giao hàng tận nơi không?
              </h3>
              <p className="text-gray-600 text-sm">
                Hiện tại chúng tôi chưa có dịch vụ giao hàng.
                Tuy nhiên, bạn có thể đặt món trước và đến nhà hàng lấy.
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardContent className="p-6">
              <h3 className="font-semibold mb-2">
                Nhà hàng có chỗ đỗ xe không?
              </h3>
              <p className="text-gray-600 text-sm">
                Có, nhà hàng có bãi đỗ xe miễn phí cho khách hàng
                với sức chứa khoảng 50 xe máy và 20 ô tô.
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardContent className="p-6">
              <h3 className="font-semibold mb-2">
                Có thể thanh toán bằng thẻ không?
              </h3>
              <p className="text-gray-600 text-sm">
                Có, chúng tôi nhận thanh toán bằng tiền mặt, thẻ ATM,
                thẻ tín dụng và các ví điện tử phổ biến.
              </p>
            </CardContent>
          </Card>
        </div>
      </section>
    </div>
  );
};