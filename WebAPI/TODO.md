One answer per user per question

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