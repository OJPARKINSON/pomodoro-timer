package cmd

import (
	"github.com/ojparkinson/pomodoro-timer/internal/session"
	"github.com/spf13/cobra"
)

var intervalTime, longBreak, shortBreak int
var name string

var startCmd = &cobra.Command{
	Use:   "start",
	Short: "Starts the pomodoro timer",
	Long:  `The pomodoro timer will start with the first focus session for a default of 25 minutes and alternate with short 5 minute breaks. Every 4 breaks there will be a long 15 minute break`,
	Run: func(cmd *cobra.Command, args []string) {
		currentSession := session.CreateSession(name, intervalTime, longBreak, shortBreak)
		currentSession.Start()
	},
}

func init() {
	rootCmd.AddCommand(startCmd)

	startCmd.Flags().IntVar(&intervalTime, "intervalTime", 25, "interval duration in minutes")
	startCmd.Flags().IntVar(&longBreak, "longBreak", 15, "long break duration in minutes")
	startCmd.Flags().IntVar(&shortBreak, "shortBreak", 5, "short break duration in minutes")
	startCmd.Flags().StringVar(&name, "name", "focus", "focus interval name")

}
