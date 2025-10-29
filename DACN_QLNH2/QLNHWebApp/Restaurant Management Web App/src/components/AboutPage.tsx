import React from 'react';
import { Card, CardContent } from './ui/card';
import { Badge } from './ui/badge';
import { Star, Award, Users, Clock, Utensils, Heart } from 'lucide-react';
import { ImageWithFallback } from './figma/ImageWithFallback';

export const AboutPage: React.FC = () => {
  const features = [
    {
      icon: <Utensils className="h-8 w-8" />,
      title: "Menu phong phú",
      description: "Hơn 100 món ăn đa dạng từ các vùng miền Việt Nam, phù hợp với mọi khẩu vị và sở thích."
    },
    {
      icon: <Users className="h-8 w-8" />,
      title: "Không gian rộng rãi",
      description: "Diện tích 500m² với thiết kế hiện đại, thoáng mát, phù hợp cho gia đình và nhóm bạn lớn."
    },
    {
      icon: <Heart className="h-8 w-8" />,
      title: "Phục vụ tận tình",
      description: "Đội ngũ nhân viên được đào tạo chuyên nghiệp, nhiệt tình và chu đáo trong từng chi tiết."
    },
    {
      icon: <Award className="h-8 w-8" />,
      title: "Chất lượng đảm bảo",
      description: "Nguyên liệu tươi ngon được chọn lọc kỹ càng, quy trình chế biến đạt chuẩn an toàn thực phẩm."
    },
    {
      icon: <Clock className="h-8 w-8" />,
      title: "Phục vụ 24/7",
      description: "Mở cửa mỗi ngày từ 10:00 - 22:00, sẵn sàng phục vụ bạn mọi lúc trong tuần."
    },
    {
      icon: <Star className="h-8 w-8" />,
      title: "Đánh giá cao",
      description: "Được hàng nghìn khách hàng tin tưởng với điểm đánh giá 4.8/5 sao trên các nền tảng."
    }
  ];

  const stats = [
    { number: "10+", label: "Năm kinh nghiệm" },
    { number: "50,000+", label: "Khách hàng hài lòng" },
    { number: "100+", label: "Món ăn đa dạng" },
    { number: "200", label: "Chỗ ngồi" }
  ];

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      {/* Hero Section */}
      <section className="text-center mb-16">
        <h1 className="text-4xl font-bold text-gray-900 mb-6">
          Về Nhà Hàng 3TL
        </h1>
        <p className="text-xl text-gray-600 max-w-3xl mx-auto mb-8">
          Chúng tôi tự hào là một trong những nhà hàng hàng đầu tại TP.HCM, 
          chuyên phục vụ các món ăn truyền thống Việt Nam với hương vị đặc trưng 
          và không gian ấm cúng, thân thiện.
        </p>
        <div className="relative max-w-4xl mx-auto">
          <ImageWithFallback
            src="https://images.unsplash.com/photo-1667388968964-4aa652df0a9b?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxyZXN0YXVyYW50JTIwaW50ZXJpb3IlMjBkaW5pbmd8ZW58MXx8fHwxNzU4MTk4ODQwfDA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral"
            alt="Không gian nhà hàng 3TL"
            className="w-full h-[400px] object-cover rounded-2xl shadow-lg"
          />
        </div>
      </section>

      {/* Stats Section */}
      <section className="mb-16">
        <div className="grid grid-cols-2 md:grid-cols-4 gap-8">
          {stats.map((stat, index) => (
            <div key={index} className="text-center">
              <div className="text-3xl font-bold text-primary mb-2">
                {stat.number}
              </div>
              <div className="text-gray-600">{stat.label}</div>
            </div>
          ))}
        </div>
      </section>

      {/* Story Section */}
      <section className="mb-16">
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-12 items-center">
          <div>
            <h2 className="text-3xl font-bold text-gray-900 mb-6">
              Câu chuyện của chúng tôi
            </h2>
            <div className="space-y-4 text-gray-600">
              <p>
                Nhà Hàng 3TL được thành lập vào năm 2014 với mục tiêu mang đến 
                những món ăn truyền thống Việt Nam chất lượng cao trong không gian 
                hiện đại và thoải mái.
              </p>
              <p>
                Từ một nhà hàng nhỏ với 20 chỗ ngồi, chúng tôi đã không ngừng phát triển 
                và mở rộng để phục vụ ngày càng nhiều thực khách. Ngày nay, Nhà Hàng 3TL 
                có thể phục vụ đồng thời 200 khách với đội ngũ hơn 30 nhân viên chuyên nghiệp.
              </p>
              <p>
                Chúng tôi luôn cam kết sử dụng nguyên liệu tươi ngon nhất, 
                giữ gìn và phát huy hương vị truyền thống trong từng món ăn, 
                đồng thời không ngừng đổi mới để mang đến trải nghiệm tuyệt vời nhất 
                cho khách hàng.
              </p>
            </div>
          </div>
          <div>
            <ImageWithFallback
              src="https://images.unsplash.com/photo-1723744895523-75a5b52de0eb?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxyZXN0YXVyYW50JTIwZm9vZCUyMGRpc2hlc3xlbnwxfHx8fDE3NTgyNDc5MzV8MA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral"
              alt="Món ăn đặc sắc"
              className="w-full h-[400px] object-cover rounded-xl shadow-lg"
            />
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section className="mb-16">
        <div className="text-center mb-12">
          <h2 className="text-3xl font-bold text-gray-900 mb-4">
            Lý do chọn Nhà Hàng 3TL
          </h2>
          <p className="text-lg text-gray-600">
            Những điều làm nên sự khác biệt của chúng tôi
          </p>
        </div>
        
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
          {features.map((feature, index) => (
            <Card key={index} className="hover:shadow-lg transition-shadow">
              <CardContent className="p-6 text-center">
                <div className="bg-primary/10 rounded-full w-16 h-16 flex items-center justify-center mx-auto mb-4 text-primary">
                  {feature.icon}
                </div>
                <h3 className="text-xl font-semibold mb-3">
                  {feature.title}
                </h3>
                <p className="text-gray-600">
                  {feature.description}
                </p>
              </CardContent>
            </Card>
          ))}
        </div>
      </section>

      {/* Mission Section */}
      <section className="mb-16 bg-gray-50 rounded-2xl p-8">
        <div className="text-center max-w-4xl mx-auto">
          <h2 className="text-3xl font-bold text-gray-900 mb-6">
            Sứ mệnh của chúng tôi
          </h2>
          <p className="text-lg text-gray-600 mb-6">
            "Mang đến những trải nghiệm ẩm thực tuyệt vời, kết nối mọi người 
            qua hương vị truyền thống Việt Nam trong không gian ấm cúng và hiện đại."
          </p>
          <div className="flex flex-wrap justify-center gap-3">
            <Badge variant="secondary" className="px-4 py-2">
              Chất lượng hàng đầu
            </Badge>
            <Badge variant="secondary" className="px-4 py-2">
              Dịch vụ tận tâm
            </Badge>
            <Badge variant="secondary" className="px-4 py-2">
              Giá trị truyền thống
            </Badge>
            <Badge variant="secondary" className="px-4 py-2">
              Đổi mới không ngừng
            </Badge>
          </div>
        </div>
      </section>

      {/* Team Section */}
      <section>
        <div className="text-center">
          <h2 className="text-3xl font-bold text-gray-900 mb-6">
            Đội ngũ của chúng tôi
          </h2>
          <p className="text-lg text-gray-600 mb-8">
            Đội ngũ hơn 30 nhân viên chuyên nghiệp, từ bếp trưởng đến nhân viên phục vụ, 
            tất cả đều được đào tạo kỹ lưỡng và có kinh nghiệm lâu năm trong ngành.
          </p>
          
          <div className="bg-primary/5 rounded-xl p-8">
            <div className="grid grid-cols-1 md:grid-cols-3 gap-8 text-center">
              <div>
                <div className="text-2xl font-bold text-primary mb-2">10+</div>
                <div className="text-gray-600">Đầu bếp chuyên nghiệp</div>
              </div>
              <div>
                <div className="text-2xl font-bold text-primary mb-2">15+</div>
                <div className="text-gray-600">Nhân viên phục vụ</div>
              </div>
              <div>
                <div className="text-2xl font-bold text-primary mb-2">5+</div>
                <div className="text-gray-600">Quản lý điều hành</div>
              </div>
            </div>
          </div>
        </div>
      </section>
    </div>
  );
};