/*
Copyright © 2025 NAME HERE <EMAIL ADDRESS>
*/
package cmd

import (
	"github.com/ojparkinson/pomodoro-timer/internal/session"
	"github.com/spf13/cobra"
)

var intervalTime, longBreak, shortBreak int

// startCmd represents the start command
var startCmd = &cobra.Command{
	Use:   "start",
	Short: "A brief description of your command",
	Long: `A longer description that spans multiple lines and likely contains examples
		and usage of using your command. For example:

		Cobra is a CLI library for Go that empowers applications.
		This application is a tool to generate the needed files
		to quickly create a Cobra application.`,
	Run: func(cmd *cobra.Command, args []string) {
		currentSession := session.CreateSession("test", intervalTime, longBreak, shortBreak)
		currentSession.Start()
	},
}

func init() {
	rootCmd.AddCommand(startCmd)

	startCmd.Flags().IntVar(&intervalTime, "intervalTime", 25, "interval duration in minutes")
	startCmd.Flags().IntVar(&longBreak, "longBreak", 15, "long break duration in minutes")
	startCmd.Flags().IntVar(&shortBreak, "shortBreak", 5, "short break duration in minutes")

}
