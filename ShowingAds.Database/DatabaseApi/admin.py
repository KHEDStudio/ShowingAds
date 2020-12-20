from django.contrib import admin
from .models import UserProfile, Channel, Device, Log, Content, Video, Order_Client, Ads_Client_Channel, Ads_Client, Showed, Ads_Video

admin.site.register(UserProfile)
admin.site.register(Channel)
admin.site.register(Content)
admin.site.register(Video)
admin.site.register(Ads_Client_Channel)
admin.site.register(Ads_Client)
admin.site.register(Ads_Video)
admin.site.register(Order_Client)
admin.site.register(Device)
admin.site.register(Log)
admin.site.register(Showed)