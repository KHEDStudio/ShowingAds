from django.http import HttpResponse
from rest_framework import status
from rest_framework.views import APIView
from django.http import JsonResponse
from django.contrib.auth.models import User
from django.contrib.auth import authenticate
from .models import UserProfile, Channel, Device, Log, Content, Video, Order_Client, Ads_Client_Channel, Ads_Client, Showed, Ads_Video
from .serializers import UserProfileSerializer, ChannelSerializer, DeviceSerializer, LogSerializer, ContentSerializer, VideoSerializer, OrderClientSerializer, AdsClientChannelSerializer, AdsClientSerializer, ShowedSerializer, AdsVideoSerializer

class SessionView(APIView):
    def post(self, request, format=None):
        username = request.data.get('username')
        password = request.data.get('password')
        user = authenticate(username=username, password=password)
        if not user is None:
            user = UserProfileSerializer(user.profile, many=False)
            return JsonResponse(user.data, status=status.HTTP_200_OK)
        return HttpResponse(status=status.HTTP_404_NOT_FOUND)

class UserProfileView(APIView):
    def get(self, request, format=None):
        users = UserProfileSerializer(UserProfile.objects.all(), many=True)
        return JsonResponse(users.data, safe=False)

class ChannelView(APIView):
    def get(self, request, format=None):
        channels = ChannelSerializer(Channel.objects.all(), many=True)
        return JsonResponse(channels.data, safe=False)

    def post(self, request, fromat=None):
        channel = ChannelSerializer(data=request.data)
        if channel.is_valid():
            channel.save()
            return JsonResponse(channel.data, status=status.HTTP_201_CREATED)
        return JsonResponse(channel.errors, status=status.HTTP_400_BAD_REQUEST)

    def put(self, request, format=None):
        try:
            channel = Channel.objects.get(id=request.data.get('id'))
        except Channel.DoesNotExist:
            channel = None
        if channel == None:
            return HttpResponse(status=status.HTTP_404_NOT_FOUND)
        channel = ChannelSerializer(channel, data=request.data)
        if channel.is_valid():
            channel.save()
            return JsonResponse(channel.data, status=status.HTTP_200_OK)
        return JsonResponse(channel.errors, status=status.HTTP_400_BAD_REQUEST)

    def delete(self, request, format=None):
        try:
            channel = Channel.objects.get(id=request.data.get('id'))
        except Channel.DoesNotExist:
            channel = None
        if channel == None:
            return HttpResponse(status=status.HTTP_404_NOT_FOUND)
        channel.delete()
        return HttpResponse(status=status.HTTP_200_OK)

class DeviceView(APIView):
    def get(self, request, format=None):
        devices = DeviceSerializer(Device.objects.all(), many=True)
        return JsonResponse(devices.data, safe=False)

    def post(self, request, format=None):
        if request.data.get('channel') == '00000000-0000-0000-0000-000000000000':
            request.data['channel'] = None
        device = DeviceSerializer(data=request.data)
        if device.is_valid():
            device.save()
            return JsonResponse(device.data, status=status.HTTP_201_CREATED)
        return JsonResponse(device.errors, status=status.HTTP_400_BAD_REQUEST)

    def put(self, request, format=None):
        try:
            device = Device.objects.get(id=request.data.get('id'))
        except Device.DoesNotExist:
            device = None
        if device == None:
            return HttpResponse(status=status.HTTP_404_NOT_FOUND)
        if request.data.get('channel') == '00000000-0000-0000-0000-000000000000':
            request.data['channel'] = None
        device = DeviceSerializer(device, data=request.data)
        if device.is_valid():
            device.save()
            return JsonResponse(device.data, status=status.HTTP_200_OK)
        return JsonResponse(device.errors, status=status.HTTP_400_BAD_REQUEST)

    def delete(self, request, format=None):
        try:
            device = Device.objects.get(id=request.data.get('id'))
        except Device.DoesNotExist:
            device = None
        if device == None:
            return HttpResponse(status=status.HTTP_404_NOT_FOUND)
        device.delete()
        return HttpResponse(status=status.HTTP_200_OK)

class LogView(APIView):
    def get(self, request, format=None):
        logs = LogSerializer(Log.objects.all(), many=True)
        return JsonResponse(logs.data, safe=False)

    def post(self, request, fromat=None):
        log = LogSerializer(data=request.data)
        if log.is_valid():
            log.save()
            return JsonResponse(log.data, status=status.HTTP_201_CREATED)
        return JsonResponse(log.errors, status=status.HTTP_400_BAD_REQUEST)

    def delete(self, request, format=None):
        try:
            log = Log.objects.get(id=request.data.get('id'))
        except Log.DoesNotExist:
            log = None
        if log == None:
            return HttpResponse(status=status.HTTP_404_NOT_FOUND)
        log.delete()
        return HttpResponse(status=status.HTTP_200_OK)

class ContentView(APIView):
    def get(self, request, format=None):
        contents = ContentSerializer(Content.objects.all(), many=True)
        return JsonResponse(contents.data, safe=False)

    def post(self, request, fromat=None):
        content = ContentSerializer(data=request.data)
        if content.is_valid():
            content.save()
            return JsonResponse(content.data, status=status.HTTP_201_CREATED)
        return JsonResponse(content.errors, status=status.HTTP_400_BAD_REQUEST)

    def put(self, request, format=None):
        try:
            content = Content.objects.get(id=request.data.get('id'))
        except Content.DoesNotExist:
            content = None
        if content == None:
            return HttpResponse(status=status.HTTP_404_NOT_FOUND)
        content = ContentSerializer(content, data=request.data)
        if content.is_valid():
            content.save()
            return JsonResponse(content.data, status=status.HTTP_200_OK)
        return JsonResponse(content.errors, status=status.HTTP_400_BAD_REQUEST)

    def delete(self, request, format=None):
        try:
            content = Content.objects.get(id=request.data.get('id'))
        except Content.DoesNotExist:
            content = None
        if content == None:
            return HttpResponse(status=status.HTTP_404_NOT_FOUND)
        content.delete()
        return HttpResponse(status=status.HTTP_200_OK)

class VideoView(APIView):
    def get(self, request, format=None):
        videos = VideoSerializer(Video.objects.all(), many=True)
        return JsonResponse(videos.data, safe=False)

    def post(self, request, fromat=None):
        id = request.data.get('id')
        duration = request.data.get('duration')
        content = Content.objects.get(id=request.data.get('content'))
        video = Video(id=id, duration=duration, content=content)
        video.save()
        return HttpResponse(status=status.HTTP_201_CREATED)

    def delete(self, request, format=None):
        try:
            video = Video.objects.get(id=request.data.get('id'))
        except Video.DoesNotExist:
            video = None
        if video == None:
            return HttpResponse(status=status.HTTP_404_NOT_FOUND)
        video.delete()
        return HttpResponse(status=status.HTTP_200_OK)

class OrderClientView(APIView):
    def get(self, request, format=None):
        orders = OrderClientSerializer(Order_Client.objects.all(), many=True)
        return JsonResponse(orders.data, safe=False)

    def post(self, request, fromat=None):
        order = OrderClientSerializer(data=request.data)
        if order.is_valid():
            order.save()
            return JsonResponse(order.data, status=status.HTTP_201_CREATED)
        return JsonResponse(order.errors, status=status.HTTP_400_BAD_REQUEST)

    def put(self, request, format=None):
        try:
            order = Order_Client.objects.get(id=request.data.get('id'))
        except Order_Client.DoesNotExist:
            order = None
        if order == None:
            return HttpResponse(status=status.HTTP_404_NOT_FOUND)
        order = OrderClientSerializer(order, data=request.data)
        if order.is_valid():
            order.save()
            return JsonResponse(order.data, status=status.HTTP_200_OK)
        return JsonResponse(order.errors, status=status.HTTP_400_BAD_REQUEST)

    def delete(self, request, format=None):
        try:
            order = Order_Client.objects.get(id=request.data.get('id'))
        except Order_Client.DoesNotExist:
            order = None
        if order == None:
            return HttpResponse(status=status.HTTP_404_NOT_FOUND)
        order.delete()
        return HttpResponse(status=status.HTTP_200_OK)

class AdsClientChannelView(APIView):
    def get(self, request, format=None):
        connections = AdsClientChannelSerializer(Ads_Client_Channel.objects.all(), many=True)
        return JsonResponse(connections.data, safe=False)

    def post(self, request, fromat=None):
        connection = AdsClientChannelSerializer(data=request.data)
        if connection.is_valid():
            connection.save()
            return JsonResponse(connection.data, status=status.HTTP_201_CREATED)
        return JsonResponse(connection.errors, status=status.HTTP_400_BAD_REQUEST)

    def put(self, request, format=None):
        try:
            connection = Ads_Client_Channel.objects.get(id=request.data.get('id'))
        except Ads_Client_Channel.DoesNotExist:
            connection = None
        if connection == None:
            return HttpResponse(status=status.HTTP_404_NOT_FOUND)
        connection = AdsClientChannelSerializer(connection, data=request.data)
        if connection.is_valid():
            connection.save()
            return JsonResponse(connection.data, status=status.HTTP_200_OK)
        return JsonResponse(connection.errors, status=status.HTTP_400_BAD_REQUEST)

    def delete(self, request, format=None):
        try:
            connection = Ads_Client_Channel.objects.get(id=request.data.get('id'))
        except Ads_Client_Channel.DoesNotExist:
            connection = None
        if connection == None:
            return HttpResponseHttpResponse(status=status.HTTP_404_NOT_FOUND)
        connection.delete()
        return HttpResponse(status=status.HTTP_200_OK)

class AdsClientView(APIView):
    def get(self, request, format=None):
        clients = AdsClientSerializer(Ads_Client.objects.all(), many=True)
        return JsonResponse(clients.data, safe=False)

    def post(self, request, fromat=None):
        client = AdsClientSerializer(data=request.data)
        if client.is_valid():
            client.save()
            return JsonResponse(client.data, status=status.HTTP_201_CREATED)
        return JsonResponse(client.errors, status=status.HTTP_400_BAD_REQUEST)

    def put(self, request, format=None):
        try:
            client = Ads_Client.objects.get(id=request.data.get('id'))
        except Ads_Client.DoesNotExist:
            client = None
        if client == None:
            return HttpResponse(status=status.HTTP_404_NOT_FOUND)
        client = AdsClientSerializer(client, data=request.data)
        if client.is_valid():
            client.save()
            return JsonResponse(client.data, status=status.HTTP_200_OK)
        return JsonResponse(client.errors, status=status.HTTP_400_BAD_REQUEST)

    def delete(self, request, format=None):
        try:
            client = Ads_Client.objects.get(id=request.data.get('id'))
        except Ads_Client.DoesNotExist:
            client = None
        if client == None:
            return HttpResponse(status=status.HTTP_404_NOT_FOUND)
        client.delete()
        return HttpResponse(status=status.HTTP_200_OK)

class ShowedView(APIView):
    def get(self, request, format=None):
        showeds = ShowedSerializer(Showed.objects.all(), many=True)
        return JsonResponse(showeds.data, safe=False)

    def post(self, request, fromat=None):
        showed = ShowedSerializer(data=request.data)
        if showed.is_valid():
            showed.save()
            return JsonResponse(showed.data, status=status.HTTP_201_CREATED)
        return JsonResponse(showed.errors, status=status.HTTP_400_BAD_REQUEST)

    def put(self, request, format=None):
        try:
            showed = Showed.objects.get(id=request.data.get('id'))
        except Showed.DoesNotExist:
            showed = None
        if showed == None:
            return HttpResponse(status=status.HTTP_404_NOT_FOUND)
        showed = ShowedSerializer(showed, data=request.data)
        if showed.is_valid():
            showed.save()
            return JsonResponse(showed.data, status=status.HTTP_200_OK)
        return JsonResponse(showed.errors, status=status.HTTP_400_BAD_REQUEST)

    def delete(self, request, format=None):
        try:
            showed = Showed.objects.get(id=request.data.get('id'))
        except Showed.DoesNotExist:
            showed = None
        if showed == None:
            return HttpResponse(status=status.HTTP_404_NOT_FOUND)
        showed.delete()
        return HttpResponse(status=status.HTTP_200_OK)

class AdsVideoView(APIView):
    def get(self, request, format=None):
        videos = AdsVideoSerializer(Ads_Video.objects.all(), many=True)
        return JsonResponse(videos.data, safe=False)

    def post(self, request, fromat=None):
        id = request.data.get('id')
        duration = request.data.get('duration')
        ads_client = Ads_Client.objects.get(id=request.data.get('ads_client'))
        video = Ads_Video(id=id, duration=duration, ads_client=ads_client)
        video.save()
        return HttpResponse(status=status.HTTP_201_CREATED)

    def delete(self, request, format=None):
        try:
            video = Ads_Video.objects.get(id=request.data.get('id'))
        except Ads_Video.DoesNotExist:
            video = None
        if video == None:
            return HttpResponse(status=status.HTTP_404_NOT_FOUND)
        video.delete()
        return HttpResponse(status=status.HTTP_200_OK)