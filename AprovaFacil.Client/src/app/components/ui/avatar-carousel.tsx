import { Carousel, CarouselContent, CarouselItem, CarouselNext, CarouselPrevious } from '@/app/components/ui/carousel'; // Adjust the import based on your setup
import { Card, CardContent } from './card';

export function AvatarCarousel({ field }) {
  // Define the image paths based on the folder structure
  const femaleImages = [
    '/avatars/female/82.png',
    '/avatars/female/86.png',
    '/avatars/female/96.png',
    '/avatars/female/99.png',
    '/avatars/female/100.png',
  ];

  const maleImages = [
    '/avatars/male/19.png',
    '/avatars/male/24.png',
    '/avatars/male/36.png',
    '/avatars/male/47.png',
    '/avatars/male/48.png',
  ];

  // Combine the images into a single array (or keep them separate if you want two carousels)
  const allImages = [...femaleImages, ...maleImages];

  // Handle image selection
  const handleImageSelect = (imagePath: string) => {
    field.onChange(imagePath);
    //console.log('Selected image:', imagePath); // You can replace this with your logic
  };

  return (
    <div>
      <Carousel
        opts={{
          align: "start",
          loop: true,
          slidesToScroll: 1,
          startIndex: 8,
        }}
        className="w-full md:max-w-2xl max-w-52"
      >
        <CarouselContent>
          {allImages.map((imagePath, index) => (
            <CarouselItem key={index} className="basis-1/2 md:basis-1/2 lg:basis-1/5">
              <div className="p-1">
                <Card className='rounded-full'>
                  <CardContent
                    className={`flex items-center justify-center p-1 cursor-pointer ${
                      field.value === imagePath ? 'border-2 border-green-500 rounded-full' : 'border border-gray-300 rounded-full'
                    }`}
                    onClick={() => handleImageSelect(imagePath)}
                  >
                    <img
                      src={imagePath}
                      alt={`Avatar ${index + 1}`}
                      className="w-full h-full rounded-full "
                      loading='lazy'
                    />
                  </CardContent>
                </Card>
              </div>
            </CarouselItem>
          ))}
        </CarouselContent>
        <CarouselPrevious />
        <CarouselNext />
      </Carousel>
    </div>
  );
};

export default AvatarCarousel;