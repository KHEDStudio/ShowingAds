import os
import uuid
from django.db import models
from django.apps import apps
from django.dispatch import receiver
from django.contrib.auth.models import User

ROLE_CHOICES = ((0, 'user'), (1, 'client'))

class UserProfile(models.Model):
    owner = models.ForeignKey(User, on_delete=models.CASCADE, blank=False)
    user = models.OneToOneField(User, on_delete=models.CASCADE, related_name="profile", blank=False)
    role = models.IntegerField(blank=False, default=1, choices=ROLE_CHOICES)
    red = models.PositiveSmallIntegerField(default=108, blank=False)
    green = models.PositiveSmallIntegerField(default=117, blank=False)
    blue = models.PositiveSmallIntegerField(default=125, blank=False)

    def __str__(self):
        return self.user.username

ORIENTATION_CHOICES = ((0, 'Горизонтальная'), (1, 'Вертикальная'))

class Channel(models.Model):
    id = models.UUIDField(primary_key=True, default=uuid.uuid4)
    account = models.ForeignKey(User, on_delete=models.CASCADE, related_name='channels_account')
    workers = models.ManyToManyField(User, blank=True, related_name='channels_worker')
    founder = models.CharField(max_length=200, blank=False)
    name = models.CharField(max_length=50, blank=False)
    description = models.CharField(max_length=500, blank=True)
    logo_left = models.UUIDField(default=uuid.uuid4, blank=True, null=True)
    logo_right = models.UUIDField(default=uuid.uuid4, blank=True, null=True)
    ticker = models.CharField(max_length=1000, blank=True)
    ticker_interval = models.BigIntegerField(default=0, blank=False)
    reload_time = models.BigIntegerField(default=0, blank=False)
    orientation = models.IntegerField(blank=False, default=1, choices=ORIENTATION_CHOICES)
    contents = models.ManyToManyField('DatabaseApi.Content', blank=True, related_name='channels')

    def __str__(self):
        return self.name

class Device(models.Model):
    id = models.UUIDField(primary_key=True, default=uuid.uuid4)
    account = models.ForeignKey(User, on_delete=models.CASCADE, related_name='devices_account')
    name = models.CharField(max_length=50, blank=False)
    address = models.CharField(max_length=500, blank=True)
    latitude = models.FloatField(blank=False, default=0)
    longitude = models.FloatField(blank=False, default=0)
    last_online = models.DateTimeField(blank=False)
    channel = models.ForeignKey('DatabaseApi.Channel', on_delete=models.SET_NULL, null=True, blank=True, related_name='devices')

    def __str__(self):
        return self.name

class Log(models.Model):
    id = models.UUIDField(primary_key=True, default=uuid.uuid4)
    device = models.ForeignKey('DatabaseApi.Device', on_delete=models.CASCADE, related_name='logs')
    title = models.CharField(max_length=100, blank=False)
    description = models.CharField(max_length=500, blank=False)
    time = models.DateTimeField(auto_now_add=True, blank=False)

class Content(models.Model):
    id = models.UUIDField(primary_key=True, default=uuid.uuid4)
    account = models.ForeignKey(User, on_delete=models.CASCADE, related_name='contents')
    founder = models.CharField(max_length=200, blank=False)
    name = models.CharField(max_length=50, blank=False)
    description = models.CharField(max_length=500, blank=True)

    def __str__(self):
        return self.name

class Video(models.Model):
    id = models.UUIDField(primary_key=True, default=uuid.uuid4)
    duration = models.BigIntegerField(default=0, blank=False)
    content = models.ForeignKey('DatabaseApi.Content', on_delete=models.CASCADE, related_name='videos')

class Order_Client(models.Model):
    id = models.UUIDField(primary_key=True, default=uuid.uuid4)
    channel = models.ForeignKey('DatabaseApi.Channel', on_delete=models.CASCADE, related_name='orders')
    ads_client_channel = models.ForeignKey('DatabaseApi.Ads_Client_Channel', on_delete=models.CASCADE, related_name='orders')
    order_field = models.DateTimeField(auto_now=True, blank=False)

class Ads_Client_Channel(models.Model):
    id = models.UUIDField(primary_key=True, default=uuid.uuid4)
    channel = models.ForeignKey('DatabaseApi.Channel', on_delete=models.CASCADE, related_name='ads_clients')
    ads_client = models.ForeignKey('DatabaseApi.Ads_Client', on_delete=models.CASCADE, related_name='ads_channels')
    ads_videos = models.ManyToManyField('DatabaseApi.Ads_Video', blank=True)
    interval = models.PositiveIntegerField(default=0, blank=False)

class Ads_Client(models.Model):
    id = models.UUIDField(primary_key=True, default=uuid.uuid4)
    account = models.ForeignKey(User, on_delete=models.CASCADE, related_name='ads_clients_account')
    client = models.OneToOneField(User, on_delete=models.SET_NULL, related_name='ads_client', null=True, blank=True)
    founder = models.CharField(max_length=200, blank=False)
    name = models.CharField(max_length=50, blank=False)
    description = models.CharField(max_length=500, blank=True)

    def __str__(self):
        return self.name

class Showed(models.Model):
    id = models.UUIDField(primary_key=True, default=uuid.uuid4)
    ads_video = models.ForeignKey('DatabaseApi.Ads_Video', on_delete=models.CASCADE, related_name='shows')
    device = models.ForeignKey(Device, on_delete=models.CASCADE)
    all_time = models.PositiveIntegerField(default=0, blank=False)
    year = models.PositiveIntegerField(default=0, blank=False)
    update_year = models.DateTimeField(blank=False)
    month = models.PositiveIntegerField(default=0, blank=False)
    update_month = models.DateTimeField(blank=False)
    week = models.PositiveIntegerField(default=0, blank=False)
    update_week = models.DateTimeField(blank=False)
    day = models.PositiveIntegerField(default=0, blank=False)
    update_day = models.DateTimeField(blank=False)
    last_time = models.DateTimeField(blank=True)
    next_time = models.DateTimeField(blank=True)

class Ads_Video(models.Model):
    id = models.UUIDField(primary_key=True, default=uuid.uuid4)
    duration = models.BigIntegerField(default=0, blank=False)
    ads_client = models.ForeignKey('DatabaseApi.Ads_Client', on_delete=models.CASCADE, related_name='videos')