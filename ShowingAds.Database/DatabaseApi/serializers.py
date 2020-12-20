from rest_framework import serializers
from .models import UserProfile, Channel, Device, Log, Content, Video, Order_Client, Ads_Client_Channel, Ads_Client, Showed, Ads_Video

class UserProfileSerializer(serializers.ModelSerializer):
    id = serializers.SerializerMethodField()
    username = serializers.SerializerMethodField()
    email = serializers.SerializerMethodField()
    last_login = serializers.SerializerMethodField()
    class Meta:
        model = UserProfile
        fields = ['id', 'red', 'green', 'blue', 'role', 'owner', 'username', 'email', 'last_login']

    def get_id(self, obj):
        return obj.user.id

    def get_username(self, obj):
        return obj.user.username

    def get_email(self, obj):
        return obj.user.email

    def get_last_login(self, obj):
        return obj.user.last_login

class ChannelSerializer(serializers.ModelSerializer):
    class Meta:
        model = Channel
        fields = '__all__'

class DeviceSerializer(serializers.ModelSerializer):
    class Meta:
        model = Device
        fields = '__all__'

class LogSerializer(serializers.ModelSerializer):
    class Meta:
        model = Log
        fields = '__all__'

class ContentSerializer(serializers.ModelSerializer):
    class Meta:
        model = Content
        fields = '__all__'

class VideoSerializer(serializers.ModelSerializer):
    class Meta:
        model = Video
        fields = '__all__'

class OrderClientSerializer(serializers.ModelSerializer):
    class Meta:
        model = Order_Client
        fields = '__all__'

class AdsClientChannelSerializer(serializers.ModelSerializer):
    class Meta:
        model = Ads_Client_Channel
        fields = '__all__'

class AdsClientSerializer(serializers.ModelSerializer):
    class Meta:
        model = Ads_Client
        fields = '__all__'

class ShowedSerializer(serializers.ModelSerializer):
    class Meta:
        model = Showed
        fields = '__all__'

class AdsVideoSerializer(serializers.ModelSerializer):
    class Meta:
        model = Ads_Video
        fields = '__all__'