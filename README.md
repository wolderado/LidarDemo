# LidarDemo

Inspired by the indie game LIDAR.exe (https://kenforest.itch.io/lidar-exe)

I wanted to publish the code so people can try their own versions. Also there aren't much code examples about DrawInstanced and material blocks. But I had to delete everything in the scene to prevent some people from outright cloning the project.

Creator of lidar.exe used Particle Systems but I used GPU instancing and manually drawing the points. Actually I were to do it again, I would do it with particle systems also. Cause its already optimized for drawing lots of small meshes. It also already has frustum culling & LODs which I had to write them from scratch. There is no need to reinvent the wheel


Project files are inside LidarTest folder
