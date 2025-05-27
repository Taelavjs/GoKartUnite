# Karter Social Media Platform

Welcome to the **Karter Social Media Platform**, a web application built with ASP.NET Core 8 MVC and designed to bring together karting enthusiasts. This platform allows users to track and share their fastest lap times, interact with fellow karters, and stay updated with the latest happenings at their favorite tracks.

### Features

#### **User Profile and Fastest Lap Times**
- Users can create and manage their profiles.
- Enter and view **fastest lap times** at recognized tracks.
- Access a history of fastest lap times and compare performance.
- Manage **friend requests** and update personal details.

#### **Friend Requests and Social Features**
- Manage friend requests and connect with other karters.
- **Meetup Requests:** Ask fellow karters to meet up and race if they are going to a track.
  
#### **Group Creation and Interaction**
- Users can create and join **groups** for sharing messages, updates, and racing-related content.
- Whenever a user submits a new lap time, a message is sent within the group, showcasing their achievement and encouraging others.

#### **Blog Posts and Community Interaction**
- Users can **create blog posts** about their racing experiences and karting tips.
- Other users can upvote and comment on blog posts.
- Posts are tagged with the track the user raced at and can be sorted by these tags.
- When a blog post is created, **friends** or **followers** of the tagged track are notified.

#### **Track Pages and Local Karter Discovery**
- The **tracks page** helps users discover local karters who race at specific tracks.
- Users can explore **tagged blog posts** related to a track and interact with content.
- If users can’t find their requested track location, they can submit a **request** to add it.
- Backend fetches results from **Google Places API** for an admin to verify and add new tracks to the verified database.

### How It Works

1. **Create and Manage Your Profile:**
   - Enter your fastest lap times and track details.
   - View a history of your lap times and see how you stack up against others.
   - Authentication made easy with Google's OAuth2 sign in. 

2. **Social Features:**
   - Send friend requests to connect with local karters and share your racing experiences.
   - Create or join **groups** to chat, share lap times, and organize races.
   - **Meetup Requests** allow you to plan race days with friends or fellow karters.

3. **Create and Interact with Blog Posts:**
   - Share racing tips, experiences, or stories in a **blog post**.
   - Comment on and upvote others' posts, and see updates from friends or tracked locations.

4. **Discover Tracks and Local Karter Community:**
   - Visit track pages to discover other karters racing there.
   - Search and view blog posts related to specific tracks.

5. **Add New Tracks:**
   - Can't find a track? Submit a request to add it to the platform.
   - Admins verify and approve new tracks from the Google Places API.
     
5. **Algorithmic Content:**
   - Ranked track recommendation system based on friends local track, distances to tracks, blog interaction and distance.
     
### Backend & API Integrations
- **Google Places API:** Used for fetching track data when users submit new locations for approval.
- **ASP.NET Core 8 MVC:** The backend is built with ASP.NET Core 8 MVC for a robust and scalable architecture.
  
---

### Clips

#### **Profile Page**
https://github.com/user-attachments/assets/a5638e17-c03b-43b1-8f9b-6bc6389ed2c8

#### **Group Interaction**
https://github.com/user-attachments/assets/071a1363-0d97-4443-9864-153971a7a96f

#### **Blog Post Interaction**
https://github.com/user-attachments/assets/dd109120-bd95-439a-be75-c9082b1e4c9e

#### **Track Pages**
https://github.com/user-attachments/assets/f2fc2261-263b-4fe1-9e4d-5a69c1297b44

---

### Tech Stack
- **Frontend:** ASP.NET Core MVC (Razor Views), Bootstrap, JavasScript
- **Backend:** ASP.NET Core 8 MVC
- **Database:** SQL Server 
- **APIs:** Google Places API for track location fetching


