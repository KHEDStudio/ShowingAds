"""
Definition of urls for ShowingAds.Database.
"""

from datetime import datetime
from django.urls import path
from django.contrib import admin
from django.contrib.auth.views import LoginView, LogoutView
from DatabaseApi import views


urlpatterns = [
    path('', admin.site.urls),
    path('session/', views.SessionView.as_view()),
    path('users/', views.UserProfileView.as_view()),
    path('channels/', views.ChannelView.as_view()),
    path('devices/', views.DeviceView.as_view()),
    path('logs/', views.LogView.as_view()),
    path('contents/', views.ContentView.as_view()),
    path('videos/', views.VideoView.as_view()),
    path('orders/', views.OrderClientView.as_view()),
    path('client_channels/', views.AdsClientChannelView.as_view()),
    path('clients/', views.AdsClientView.as_view()),
    path('showeds/', views.ShowedView.as_view()),
    path('ads_videos/', views.AdsVideoView.as_view()),
]
