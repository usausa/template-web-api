UPDATE Data SET Name = /*@ name */'', Value = /*@ value */0, Version = Version + 1
WHERE Id = /*@ id */0 AND ((/*@ version */0 IS NULL) OR (Version = /*@ version */0))
RETURNING Version
