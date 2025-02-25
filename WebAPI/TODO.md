Question bounty

Implement user is banned check

Impl Refresh Token (Add col to AppUser, Change Expr min back to 15)

FInd soft delete entity by check IsDeleted ("IsBanned")

Add Bloom Filter for email dup validation

Elastic search for question search

chec question is cloase when add comment and answer

SELECT TOP 15000
    t.TagName,
    COALESCE(ep.Body, '') AS Excerpt,
    COALESCE(wp.Body, '') AS WikiBody
FROM Tags t
    -- Join to get the excerpt post (if any)
    LEFT JOIN Posts ep ON ep.Id = t.ExcerptPostId
    -- Join to get the full wiki post (if any)
    LEFT JOIN Posts wp ON wp.Id = t.WikiPostId
ORDER BY t.Count DESC;

1. User Management:
- View all users and their details
- Manage user roles (Admin/Moderator/Regular User)
- Ban/Unban users
- View user reputation history
- Reset user passwords
- View user activity logs
2. Content Moderation:
- View reported questions/answers/comments
- Delete/Edit inappropriate content
- Lock/Unlock questions
- Monitor spam detection
- View content edit history
3. Tag Management:
- Create/Edit/Delete tags
- Merge similar tags
- View tag usage statistics
- Manage tag synonyms
- Set tag restrictions
4. Analytics Dashboard:
- User engagement metrics
- Popular questions/tags
- Site performance metrics
- User growth trends
- Content quality metrics
5. System Configuration:
- Manage reputation thresholds
- Configure site settings
- Manage API rate limits
- Configure caching policies