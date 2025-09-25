package session

import (
	"fmt"
	"time"
)

type Session struct {
	createdAt    time.Time
	name         string
	mode         string
	intervalTime time.Duration
	shortBreak   time.Duration
	longBreak    time.Duration
	cycleCount   int
	maxCycles    int
	startTime    time.Time
	currentEnd   time.Time
	isRunning    bool
	isBreak      bool
	isPaused     bool
}

func CreateSession(name string, intervalTime int, shortBreak int, longBreak int) *Session {
	return &Session{
		createdAt:    time.Now(),
		name:         name,
		mode:         "basic",
		intervalTime: time.Duration(60-intervalTime) * time.Minute,
		longBreak:    time.Duration(60-longBreak) * time.Minute,
		shortBreak:   time.Duration(60-shortBreak) * time.Minute,
	}
}

func (s *Session) Start() error {
	quit := make(chan bool)
	pause := make(chan bool)
	skip := make(chan bool)

	s.isRunning = true

	ticker := time.NewTicker(time.Second)
	defer ticker.Stop()

	s.isRunning = true

	for s.isRunning {
		select {
		case <-quit:
			s.isRunning = false
			return nil
		case <-pause:
			s.togglePause()
		case <-skip:
			s.skipInterval()
		case <-ticker.C:
			if !s.isPaused {
				s.updateDisplay()
				if time.Now().After(s.currentEnd) {
					if !s.nextInterval() {
						s.isRunning = false
						return nil
					}
				}
			}
		}
	}

	return nil
}

func (s *Session) togglePause() {

}

func (s *Session) skipInterval() {

}
func (s *Session) updateDisplay() {
	fmt.Println(time.Since(s.currentEnd).Seconds())

}
func (s *Session) nextInterval() bool {
	return true
}
