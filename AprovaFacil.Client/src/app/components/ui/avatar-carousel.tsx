"use client"

import { useEffect, useState } from 'react';
import { Carousel, CarouselContent, CarouselItem, CarouselNext, CarouselPrevious, type CarouselApi } from '@/app/components/ui/carousel'; // Adjust the import based on your setup
import { Card, CardContent } from './card';
import { ChevronUp } from 'lucide-react';

export function AvatarCarousel({ field }) {

  const [api, setApi] = useState<CarouselApi>()
  const [currentIndex, setCurrentIndex] = useState(0); // Começa no índice 8 conforme seu startIndex

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

  const initialIndex = field.value && allImages.includes(field.value)
    ? allImages.indexOf(field.value)
    : 8; // Default para 8 se não houver valor ou não encontrado

  useEffect(() => {
    if (!api) return;

    // Listener para quando o scroll mudar
    const handleScroll = () => {
      const current = api.selectedScrollSnap();
      setCurrentIndex(current);
      field.onChange(allImages[current]);
      field.value = allImages[current];
    };

    api.on('select', handleScroll);
    
    // Define a imagem inicial baseada no field.value
    api.scrollTo(initialIndex);

    // Cleanup
    return () => {
      api.off('select', handleScroll);
    };
  }, [api, initialIndex]);

  // Handle image selection
  const handleImageSelect = (imagePath: string, index: number) => {
    field.onChange(imagePath);
    api?.scrollTo(index);
  };

  return (
    <div>
      <Carousel
        opts={{
          align: "center",
          loop: true,
          slidesToScroll: 1,
          startIndex: 8,
        }}
        setApi={setApi}
        className="w-full md:max-w-2xl max-w-52"
      >
        <CarouselContent>
          {allImages.map((imagePath, index) => (
            <CarouselItem key={index} className="basis-1/2 md:basis-1/2 lg:basis-1/5">
              <div className="p-1">
                <Card className='rounded-full'>
                  <CardContent
                    className={`flex items-center justify-center p-1 cursor-pointer ${field.value === imagePath ? 'border-2 border-green-500 rounded-full' : 'border border-gray-300 rounded-full'
                      }`}
                    onClick={() => handleImageSelect(imagePath, index)}
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
      <div className='w-full flex justify-center'>
          <ChevronUp />
      </div>
    </div>
  );
};

export default AvatarCarousel;