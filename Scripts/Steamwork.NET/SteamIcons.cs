using UnityEngine;
using System.Collections;
using Steamworks;
using System.Text;
using System.Collections.Generic;

public static class SteamIcons
{
    private static Dictionary<ulong, Texture2D> steamIcons;

    static SteamIcons()
    {
        steamIcons = new Dictionary<ulong, Texture2D>();
    }

    public static Texture2D GetMySteamIcon()
    {
        return GetSteamIcon(SteamUser.GetSteamID());
    }
    public static Texture2D GetSteamIcon(int friendIndex, EFriendFlags friendFlags)
    {
        CSteamID id;
        id = SteamFriends.GetFriendByIndex(friendIndex, friendFlags);

        return GetSteamIcon(id);
    }
    private static Texture2D GetSteamIcon(CSteamID id)
    {
        if (steamIcons.ContainsKey(id.m_SteamID))
        {
            return steamIcons[id.m_SteamID];
        }

        int avatarId = SteamFriends.GetMediumFriendAvatar(id);
        uint w, h;
        int width, height;
        SteamUtils.GetImageSize(avatarId, out w, out h);
        width = (int)w;
        height = (int)h;
        int imageSize = width * height * 4;
        byte[] imageData = new byte[imageSize];

        var t = new Texture2D(width, height);
        if (SteamUtils.GetImageRGBA(avatarId, imageData, imageSize))
        {
            Color[] pixelData = new Color[width * height];

            for (int i = 0, j = 0; i < imageData.Length; i += 4, j++)
            {
                byte R, G, B, A;
                R = imageData[i + 0];
                G = imageData[i + 1];
                B = imageData[i + 2];
                A = imageData[i + 3];
                pixelData[j].r = (float)R / 255f;
                pixelData[j].g = (float)G / 255f;
                pixelData[j].b = (float)B / 255f;
                pixelData[j].a = (float)A / 255f;
            }

            System.Array.Reverse(pixelData, 0, pixelData.Length);
            t.SetPixels(pixelData);

            for (int y = 0; y < height; y++)
            {
                for (int i = 0, j = width - 1; i < width / 2; i++, j--)
                {
                    Color left = t.GetPixel(i, y);
                    t.SetPixel(i, y, t.GetPixel(j, y));
                    t.SetPixel(j, y, left);
                }
            }

            t.Apply();
        }
        else
        {
            Debug.LogError("Image Load Failed");
        }
        steamIcons[id.m_SteamID] = t;
        return t;
    }
}
