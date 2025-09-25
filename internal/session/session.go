package session

import (
	"fmt"
	"time"
)

type Session struct {
	createdAt    time.Time
	name         string
	interval     string
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
	intervalNum  int
}

func CreateSession(name string, intervalTime int, shortBreak int, longBreak int) *Session {
	return &Session{
		createdAt:    time.Now(),
		name:         name,
		interval:     name,
		intervalTime: time.Duration(intervalTime) * time.Minute,
		longBreak:    time.Duration(longBreak) * time.Minute,
		shortBreak:   time.Duration(shortBreak) * time.Minute,
	}
}

func (s *Session) Start() error {
	quit := make(chan bool)
	pause := make(chan bool)
	skip := make(chan bool)

	s.isRunning = true
	s.currentEnd = time.Now().Add(s.intervalTime)
	s.intervalNum = 1

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
					s.setNextInterval()
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
	fmt.Printf("%s left of session: %s\n", time.Until(s.currentEnd).Truncate(time.Second), s.name)

}
func (s *Session) nextInterval() bool {
	return true
}

func (s *Session) setNextInterval() {
	if s.isBreak {
		s.isBreak = false
		s.currentEnd = time.Now().Add(s.intervalTime)
		s.interval = s.name
		s.intervalNum += 1
	} else {
		if s.intervalNum%4 == 0 {
			s.interval = "long break"
			s.currentEnd = time.Now().Add(s.longBreak)
			s.isBreak = true
		} else {
			s.interval = "short break"
			s.currentEnd = time.Now().Add(s.shortBreak)
			s.isBreak = true
		}
	}
}
