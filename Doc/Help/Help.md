# CGAdvanced2017

## Содержание

* [Сцена](#Сцена)
* [Выбор файла данных для отображения](#Выбор-файла-данных-для-отображения)
* [Режимы отображения](#Режимы-отображения)
* [X-ray](#x-ray)
* [Volume rendering](#volume-rendering)
* [Isosurface](#isosurface)
* [Clip planes](#clip-planes)

## Сцена

Сцена выглядит так:

<img
  src="img/scene.PNG"
/>

В приложении реализована сферическая камера: при движении мыши с зажатой левой кнопкой можно менять положение камеры, 
с помощью колеса - приближаться/отдаляться. 

Также можно вращать объект при движении мыши с зажатой правой кнопкой.

В центре сверху отображется FPS.
В правом нижнем углу кнопка помощи, нажав на которую получим информацию о клавишах, на нажатие которых реагирует приложение.

<img
  src="img/help.PNG"
/>

F - включить/отключить FPS.

L - включить/отключить консоль.

W - включить/отключить отладочное окно.

## Выбор файла данных для отображения

В правом углу выпадающий список, содержащий несколько pvm-файлов, один из которых можно выбрать для отображения на экране.

<img
  src="img/texture_file.PNG"
/>

## Режимы отображения

В левом углу выпадающий список, в котором можно выбрать режим отображения данных из текстуры.

<img
  src="img/mode.PNG"
/>

В приложении реализованы 3 режима: x-ray, volume rendering и isosurface. Под выпадающим списком с выбором режима - выбор параметров для этого режима.

### X-ray

Для x-ray - выбор цвета (3 компоненты):

<img
  src="img/x-ray.PNG"
/>

### Volume rendering

Для volume rendering - выбор transfer function:

<img
  src="img/volume_rendering.PNG"
/>

### Isosurface

Для isosurface - выбор порогового значения и параметров для модели освещения Фонга 
(ambient, diffuse, specular color - по 3 компоненты цвета, 
shininess - характеризует гладкость поверхности - чем меньше значение, тем больше блики):

<img
  src="img/isosurface.PNG"
/>

<img
  src="img/shininess.PNG"
/>

## Clip planes

Clip planes - клиповочные плоскости, которые обрезают объект плоскостью параллельной одной из координатных плоскостей в локальной системе координат.
Если напротив соответствующей оси координат стоит галочка, то можем двигать значение слайдера - менять соответствующую координату - данные, 
со значением этой координаты меньше значения слайдера, не будут отрисовываться. 
Если галочку убрать, то объект будет прорисовываться целиком (по этой координате).

<img
  src="img/clip_planes.PNG"
/>

Baby.pvm в режиме x-ray:

<img
  src="img/x-ray_clipX.PNG"
/>

При клиповке в режиме isosurface отображается серым цветом срез данных по клиповочной плоскости.
Baby.pvm в режиме isosurface:

<img
  src="img/isosurface_clipX.PNG"
/>
