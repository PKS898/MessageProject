CREATE TABLE IF NOT EXISTS messages (
    id SERIAL PRIMARY KEY,
    sequence_number INT NOT NULL,
    text VARCHAR(128) NOT NULL,
    timestamp TIMESTAMP NOT NULL DEFAULT now()
);

CREATE INDEX idx_timestamp ON messages(timestamp);
